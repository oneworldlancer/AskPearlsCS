using System.Dynamic;
using System.Text.Json;

namespace AskPearlsCS_DOMAIN.app_UtilityManager
{
    public class AppUtility_JsonManager
    {
  public static void PopulateExpando ( dynamic expando , JsonElement element )
        {
            foreach (var property in element.EnumerateObject ( ))
            {
                if (property.Value.ValueKind == JsonValueKind.Object)
                {
                    var nestedExpando = new ExpandoObject ( );
                    PopulateExpando ( nestedExpando , property.Value );
                    ((IDictionary<string , object>) expando)[ property.Name ] = nestedExpando;
                }
                else if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    var list = new List<object> ( );
                    foreach (var item in property.Value.EnumerateArray ( ))
                    {
                        if (item.ValueKind == JsonValueKind.Object)
                        {
                            var nestedExpando = new ExpandoObject ( );
                            PopulateExpando ( nestedExpando , item );
                            list.Add ( nestedExpando );
                        }
                        else
                        {
                            list.Add ( item.ToString ( ) );
                        }
                    }
                    ((IDictionary<string , object>) expando)[ property.Name ] = list;
                }
                else
                {
                    ((IDictionary<string , object>) expando)[ property.Name ] = property.Value.ToString ( );
                }
            }
        }

    }
}
