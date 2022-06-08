using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mime;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace coursework.Addons;

public static class SqlInteraction
{
    private static readonly SqlConnection Connection =
        new (@"Server=tcp:35.232.43.131;Database=coursework;User ID=sqlserver;Password=Ks6362");
    
    static SqlInteraction() => Connection.Open();
    public static void ExitCon() => Connection.Close();

    private static SqlCommand CreateCommand(string com) => new SqlCommand(com, Connection);

    private static SqlCommand CreateProcedure(string text, List<object> values)
    { 
        text += " "+string.Join(", ", Enumerable.Range(0, values.Count).Select(x => "@" + x))+";";
        return SqlConnectParameters(text, values);
    }

    private static SqlCommand CreateFunction(string text, List<object> values)
    {
        text +=  " ("+string.Join(", ", Enumerable.Range(0, values.Count).Select(x => "@" + x)) + ");";
        return SqlConnectParameters(text, values);
    }
    
    private static SqlCommand SqlConnectParameters(string text, List<object> values)
    {
        SqlCommand command = new SqlCommand(text, Connection);
        
        for (int i = 0; i < values.Count; i++)
            command.Parameters.AddWithValue("@" + i, values[i]);
        return command;
    }
    
    public static void UpdateExecute(string table, string com2, List<(string, object)> values)
    {
        string temp = "";
        for(int i = 0; i < values.Count - 1; i++)
            temp += $"{values[i].Item1} = '{values[i].Item2}', ";
        temp += $"{values[^1].Item1} = '{values[^1].Item2}' ";
        CreateCommand("update " + table + " set " + temp + com2).ExecuteNonQuery();
    }

    public static T GetOneObject<T>(string com, List<object> values) =>
        (T)  CreateFunction(com, values).ExecuteScalar();
    
    public static void ExecuteInput(string com, List<object> values) =>
        CreateProcedure(com, values).ExecuteNonQuery();
    public static void FillTable(string com, DataTable dataTable)
    {
        SqlDataAdapter ad = new SqlDataAdapter(com, Connection);
        ad.Fill(dataTable);
    }
}