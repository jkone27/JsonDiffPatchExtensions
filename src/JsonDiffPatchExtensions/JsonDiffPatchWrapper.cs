namespace JsonDiffPatchExtensions;

using System;
using System.Collections.Generic;
using System.Linq;
using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;


public static class JsonDiffPatchWrapper
{
    private static readonly JsonDiffPatch JsonDiff = new JsonDiffPatch();

    public static JToken? DiffIgnoreProperties(
        OldResult oldResult, 
        NewResult newResult,
        Action<Exception>? loggingAction = null,
        params string[] ignoreProperties) =>
            Diff(oldResult, newResult, loggingAction, ignoreProperties, Array.Empty<string>());

    public static JToken? DiffIgnoreCasingInValuesForProperties(
        OldResult oldResult, 
        NewResult newResult,
        Action<Exception>? loggingAction = null,
        params string[] ignoreCasingForProperties) =>
            Diff(oldResult, newResult, loggingAction, Array.Empty<string>(), ignoreCasingForProperties);


    private static readonly JToken OldOnly = 
        JToken.FromObject(new { Only = "OLD" });
    private static readonly JToken NewOnly = 
        JToken.FromObject(new { Only = "NEW" });

    public static JToken? Diff(
        OldResult oldResultWrapper,
        NewResult newResultWrapper,
        Action<Exception>? loggingAction = null,
        string[]? ignoreProperties = null,
        string[]? ignoreCasingForProperties = null)
    {
        try {
            
            var newResult = newResultWrapper.Value;
            var oldResult = oldResultWrapper.Value;

            if (newResult is null && oldResult is null)
            {
                return null;
            }

            if (newResult is null)
            {
                return OldOnly;
            }

            if (oldResult is null)
            {
                return NewOnly;
            }

            var oldToken = JToken.FromObject(oldResult);
            var newToken = JToken.FromObject(newResult);

            NormalizeNullAndEmptyStrings(oldToken);
            NormalizeNullAndEmptyStrings(newToken);

            var diff = JsonDiff.Diff(oldToken, newToken);

            if (diff is null)
            {
                return null;
            }

            if (ignoreProperties?.Any() == true)
            {
                ApplyPropertyIgnore(ignoreProperties, diff);
            }

            if (ignoreCasingForProperties?.Any() == true)
            {
                ApplyCasingIgnore(ignoreCasingForProperties, diff);
            }

            if (diff?.FirstOrDefault() is null)
            {
                return null;
            }

            return diff;
        }
        catch (Exception ex)
        {
            loggingAction(ex);
        }
        
        return null;
    }

    private static void ApplyCasingIgnore(string[]? ignoreCasingForProperties, JToken? diff)
    {
        foreach (var uncasedProp in ignoreCasingForProperties ?? Array.Empty<string>())
        {
            var selected = diff?.SelectTokens($"..{uncasedProp}").ToArray() ?? Array.Empty<JToken>();

            var uniquePropertyValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var prop in selected)
            {
                var firstChildren = prop.Children().FirstOrDefault()?.Children().FirstOrDefault();
                if (firstChildren is not null)
                {
                    foreach (var v in firstChildren.Values<string>())
                    {
                        if (v is not null)
                        {
                            uniquePropertyValues.Add(v);
                        }
                    }
                }
            }

            if (uniquePropertyValues.Count <= 1)
            {
                selected.ToList().ForEach(t => t.Parent?.Remove()); // remove properties  
            }
        }
    }

    private static void ApplyPropertyIgnore(string[]? ignoreProperties, JToken? diff)
    {
        foreach (var property in ignoreProperties ?? Array.Empty<string>())
        {
            var selected = diff?.SelectTokens($"..{property}").ToArray() ?? Array.Empty<JToken>();

            foreach (var match in selected)
            {
                IfEmtpyObjectRecurse(match);
            }

            foreach (var match in selected)
            {
                match?.Parent?.Remove();
            }

        }
    }

    private static void IfEmtpyObjectRecurse(JToken? match, int depth = 0)
    {
        if (depth >= 5)
        {
            return;
        }

        var parent1 = match?.Parent?.Parent as JObject;

        if (parent1 is null)
        {
            return;
        }

        if (parent1.Properties().Count() == 1)
        {
            // go up tree to check...
            IfEmtpyObjectRecurse(parent1, depth + 1);
            
            if (parent1.Parent is JProperty)
            {
                // we can remove also parent
                parent1?.Parent?.Remove();
            }
        }
    }

    public static void NormalizeNullAndEmptyStrings(JToken token)
    {
        if (token.Type == JTokenType.Object)
        {
            foreach (JProperty property in token.Children<JProperty>())
            {
                NormalizeNullAndEmptyStrings(property.Value);
            }
        }
        else if (token.Type == JTokenType.Array)
        {
            foreach (JToken child in token.Children())
            {
                NormalizeNullAndEmptyStrings(child);
            }
        }
        else if (token.Type == JTokenType.String)
        {
            if (string.IsNullOrEmpty(token.Value<string>()))
            {
                token.Replace(JValue.CreateNull());
            }
        }
    }

}