using System.Xml;
using System.Data;
namespace workshop.Models
{
    public class DataTableString : DataTable
    {
        public string GetStringItem(){
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public DataTableString(DataTable resp){
           

            if(this.Columns.Count==0){
                if(resp.Columns.Count>0){
                    for(int i=0; i<=resp.Columns.Count-1; i++){
                        this.Columns.Add(resp.Columns[i].ColumnName);
                    }
                }
            }
                for(int i=0; i<=resp.Rows.Count-1; i++){
                    DataRow dr = this.NewRow();
                    for(int j=0; j<=resp.Columns.Count-1; j++){
                        dr[resp.Columns[j].ColumnName] = resp.Rows[i][resp.Columns[j].ColumnName].ToString();
                    }
                    this.Rows.Add(dr);
                }

        }
        
    }
}