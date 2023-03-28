namespace AMCS.Data.Util.Collections.Repositioning
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public class RepositionHistoryEntry
  {
    public IList<RepositionHistoryItem> Items { get; set; }

    public RepositionHistoryEntry()
    {
      Items = new List<RepositionHistoryItem>();
    }
  }

  public class RepositionHistoryItem
  {
    public object Item { get; set; }
    public int OldPosition { get; set; }
    public int NewPosition { get; set; }
    public int? OldOriginalPosition { get; set; }
    public int? NewOriginalPosition { get; set; }
    public int? OldGroupNo { get; set; }
    public int? OldOriginalGroupNo { get; set; }
    
    public bool IsDelete { get; set; }
  }
}
