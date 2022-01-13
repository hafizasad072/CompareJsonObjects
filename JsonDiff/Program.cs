using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using JsonDiff;

public class Program
{
    public static void Main()
    {
        var a = JObject.Parse("{\"model\": \"RRUS - 11\", \"qty\": \"4\", \"qty1\": \"4\"}");
        var b = JObject.Parse("{\"model\": \"RRUS - 11\", \"qty\": \"5\",\"qty1\": \"15\"}");
        List<AuditFields> auditHistoryFields = new List<AuditFields>();
        string[] propsArray = new[] { "qty" }; // define all props names whom changes you wanna capture.
        JsonHelper<AuditFields>.CompareObjects(a, b, propsArray, auditHistoryFields);
        Console.WriteLine(JsonConvert.SerializeObject(auditHistoryFields, Formatting.Indented));
        Console.ReadLine();
    }
}