using System.Collections.Generic;
public class RowObject
{
    public List<FieldObject> Fields;
    public string ParentRowId;
    public string RowAction;
    public string RowId;
    public RowObject()
    {
        this.Fields = new List<FieldObject>();
    }

    public RowObject(string ParentRowId, string RowId, string RowAction)
    {
        this.ParentRowId = ParentRowId;
        this.RowId = RowId;
        this.RowAction = RowAction;
        this.Fields = new List<FieldObject>();
    }
}