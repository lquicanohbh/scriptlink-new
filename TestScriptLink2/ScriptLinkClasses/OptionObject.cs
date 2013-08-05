using System.Collections.Generic;
using System.Linq;

public class OptionObject
{
    public string EntityID;
    public double EpisodeNumber;
    public double ErrorCode;
    public string ErrorMesg;
    public string Facility;
    public List<FormObject> Forms;
    public string OptionId;
    public string OptionStaffId;
    public string OptionUserId;
    public string SystemCode;
    public OptionObject()
    {
        this.Forms = new List<FormObject>();
    }
    public OptionObject(string EntityID,
        double EpisodeNumber,
        string Facility,
        string OptionId,
        string OptionStaffId,
        string OptionUserId,
        string SystemCode)
    {
        this.EntityID = EntityID;
        this.EpisodeNumber = EpisodeNumber;
        this.Facility = Facility;
        this.OptionId = OptionId;
        this.OptionStaffId = OptionStaffId;
        this.OptionUserId = OptionUserId;
        this.SystemCode = SystemCode;
        this.Forms = new List<FormObject>();
    }
}