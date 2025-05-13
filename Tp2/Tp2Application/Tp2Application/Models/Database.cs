using System.Collections.ObjectModel;
using System.Data;
using Newtonsoft.Json;
using NpgsqlTypes;

namespace Tp2Application.Model; 
using Npgsql;

public static class Database {
    private static string connectionString;
    private static NpgsqlDataSource _dataSource;
    
    public static string ConnectionString {
        get => connectionString;
        set {
            connectionString = value;
            _dataSource = NpgsqlDataSource.Create(connectionString);
        }
    }
    
    public static ObservableCollection<Knowledge> GetKnowledges(ObservableCollection<Knowledge> knowledges) {
        var cmd = _dataSource.CreateCommand("SELECT * FROM knowledges");
        var reader = cmd.ExecuteReader();
        while (reader.Read()) {
            knowledges.Add(new Knowledge()
            {
                id = reader.GetInt32(0),
                title = reader.GetString(1),
                description = reader.GetString(2),
                data = JsonConvert.DeserializeObject<OptionalInfo>(reader.GetFieldValue<string>(3))
            });
        }
        reader.Close();
        return knowledges;
    }
    
    public static ObservableCollection<Knowledge> SearchKnowledges(ObservableCollection<Knowledge> knowledges, string search) {
        var cmd = _dataSource.CreateCommand("SELECT * FROM public.knowledges WHERE knowledges_index_col @@ plainto_tsquery('french', $1)");
        if (search.ToLower() == "oui") {
            search = "true";
        } else if (search.ToLower() == "non") {
            search = "false";
        }
        cmd.Parameters.AddWithValue(search + ":*");
        var reader = cmd.ExecuteReader();
        while (reader.Read()) {
            knowledges.Add(new Knowledge()
            {
                id = reader.GetInt32(0),
                title = reader.GetString(1),
                description = reader.GetString(2),
                data = JsonConvert.DeserializeObject<OptionalInfo>(reader.GetFieldValue<string>(3))
            });
        }
        reader.Close();
        return knowledges;
    }
    
    public static ObservableCollection<Knowledge> AdvanceSearchKnowledges(ObservableCollection<Knowledge> knowledges, string search, string[] champs) {
        NpgsqlCommand cmd;
        if (champs.Length == 1) {
            if (champs[0] == "titre") {
                cmd = _dataSource.CreateCommand("SELECT * FROM public.knowledges WHERE title ILIKE $1");
            } else if (champs[0] == "description") {
                cmd = _dataSource.CreateCommand("SELECT * FROM public.knowledges WHERE description ILIKE $1");
            } else {
              cmd = _dataSource.CreateCommand("SELECT * FROM public.knowledges WHERE data->>$1 ILIKE $2"); 
              cmd.Parameters.AddWithValue(champs[0]);
            }

            if (search.ToLower() == "oui") {
                search = "true";
            } else if (search.ToLower() == "non") {
                search = "false";
            }
            
            cmd.Parameters.AddWithValue("%" + search + "%");
        }
        else if (champs.Length == 2) {
            cmd = _dataSource.CreateCommand("SELECT * FROM public.knowledges WHERE data->$1->>$2 ILIKE $3");
            cmd.Parameters.AddWithValue(champs[0]);
            cmd.Parameters.AddWithValue(champs[1]);
            cmd.Parameters.AddWithValue("%" + search + "%"); 
        }
        else {
            return knowledges;
        }
        
        var reader = cmd.ExecuteReader();
        while (reader.Read()) {
            knowledges.Add(new Knowledge()
            {
                id = reader.GetInt32(0),
                title = reader.GetString(1),
                description = reader.GetString(2),
                data = JsonConvert.DeserializeObject<OptionalInfo>(reader.GetFieldValue<string>(3))
            });
        }
        reader.Close();
        return knowledges;
    }
    
    public static void DeleteKnowledge(Knowledge knowledge) {
        var cmd = _dataSource.CreateCommand("DELETE FROM public.knowledges WHERE id = $1");
        cmd.Parameters.AddWithValue(knowledge.id);
        cmd.ExecuteNonQuery();
    }
    
    public static void UpdateKnowledge(Knowledge knowledge) {
        var cmd = _dataSource.CreateCommand("UPDATE public.knowledges SET title = $1, description = $2, data = $3 WHERE id = $4");
        cmd.Parameters.AddWithValue(knowledge.title);
        cmd.Parameters.AddWithValue(knowledge.description);
        cmd.Parameters.AddWithValue(NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(knowledge.data));
        cmd.Parameters.AddWithValue(knowledge.id);
        cmd.ExecuteNonQuery();
    }
    
    public static void AddKnowledge(Knowledge knowledge) {
        var cmd = _dataSource.CreateCommand("INSERT INTO public.knowledges (title, description, data) VALUES ($1, $2, $3)");
        cmd.Parameters.AddWithValue(knowledge.title);
        cmd.Parameters.AddWithValue(knowledge.description);
        cmd.Parameters.AddWithValue(NpgsqlDbType.Jsonb, JsonConvert.SerializeObject(knowledge.data));
        cmd.ExecuteNonQuery();
    }
}