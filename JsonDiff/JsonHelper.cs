using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonDiff
{
    public class JsonHelper<T> where T : class
{
    /// <summary>
    /// Deep compare two NewtonSoft JObjects. If they don't match, returns mismathed props.
    /// </summary>
    /// <param name="source">DB values befor change</param>
    /// <param name="target">DB values afetr change</param>
    /// <param name="auditHistoryFields">Pass by refrence List<AuditHistoryFields> to  capture fileds changes.</param>
    public static void CompareObjects(JObject source, JObject target, string[] propsArray, List<T> auditHistoryFields)
    {
        foreach (KeyValuePair<string, JToken> sourcePair in source)
        {
            switch (sourcePair.Value.Type)
            {
                case JTokenType.Object:
                    var sourceObj = filterProps(sourcePair.Value, propsArray);
                    var targetObj = filterProps(target.GetValue(sourcePair.Key), propsArray);
                    if (!(target.GetValue(sourcePair.Key) == null && target.GetValue(sourcePair.Key).Type != JTokenType.Object))
                        CompareObjects(sourceObj, targetObj, propsArray, auditHistoryFields);
                    break;
                case JTokenType.Array:
                    if (target.GetValue(sourcePair.Key) != null)
                        CompareArrays(sourcePair.Value.ToObject<JArray>(), target.GetValue(sourcePair.Key).ToObject<JArray>(), propsArray, auditHistoryFields);
                    break;
                default:
                    JToken expected = sourcePair.Value;
                    var actual = target.SelectToken(sourcePair.Key);
                    if (actual != null)
                    {
                        if (!JToken.DeepEquals(expected, actual))
                        {
                            T objAuditHistoryFields = (T)Activator.CreateInstance(typeof(T), new Object[] {
                                    sourcePair.Key,
                                    sourcePair.Value.ToString(),
                                    target.Property(sourcePair.Key).Value.ToString()});

                            auditHistoryFields.Add(objAuditHistoryFields);
                        }
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Deep compare two NewtonSoft JArray. If they don't match, returns mismathed props for each object.
    /// </summary>
    /// <param name="source">DB values befor change</param>
    /// <param name="target">DB values afetr change</param>

    private static void CompareArrays(JArray source, JArray target, string[] propsArray, List<T> auditHistoryFields)
    {
        for (var index = 0; index < source.Count; index++)
        {
            var expected = source[index];
            switch (expected.Type)
            {
                case JTokenType.Object:
                    var expectedObj = filterProps(expected, propsArray);
                    var targetObj = filterProps(target[index], propsArray);
                    var actual = (index >= target.Count) ? new JObject() : targetObj;
                    CompareObjects(expectedObj, actual, propsArray, auditHistoryFields);
                    break;
            }
        }
    }

    static Func<JToken, string[], JObject> filterProps = delegate (JToken source, string[] propsArray)
    {
        return new JObject(source.ToObject<JObject>().Properties().Where(x => propsArray.Any(y => y == x.Name)));
    };
}
}
