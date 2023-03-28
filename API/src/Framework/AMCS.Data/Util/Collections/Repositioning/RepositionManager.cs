namespace AMCS.Data.Util.Collections.Repositioning
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public enum RepositionType
  {
    PasteAbove,
    PasteBelow,
    SpecificPosition,
    Reverse
  }

  public class RepositionManager
  {
    public static void Reposition(RepositionType type, ref IList<object> originalList, ref IList<object> itemsToMove, object selectedObject, int position)
    {
      IList<object> newList = new List<object>();

      if (type == RepositionType.PasteAbove || type == RepositionType.PasteBelow)
      {
        for (int i = 0; i < originalList.Count; i++)
        {
          var originalItem = originalList[i];
          bool alreadyAdded = false;

          if (originalItem == selectedObject)
          {
            if (type == RepositionType.PasteBelow && !itemsToMove.Contains(originalItem))
            {
              newList.Add(originalItem);
              alreadyAdded = true;
            }

            foreach (var item in itemsToMove)
            {
              newList.Add(item);
            }
          }

          if (!itemsToMove.Contains(originalItem) && !alreadyAdded)
            newList.Add(originalItem);
        }
      }
      else if(type == RepositionType.SpecificPosition)
      {
        int positionStart = Math.Max(position, 1);
        bool addToEnd = positionStart >= originalList.Count || positionStart > originalList.Count - itemsToMove.Count;
        for (int i = 0; i < originalList.Count; i++)
        {
          if (newList.Count == positionStart - 1 && !addToEnd)
          {
            foreach (var item in itemsToMove)
            {
              newList.Add(item);
            }
          }

          var originalItem = originalList[i];
          if (!itemsToMove.Contains(originalItem))
            newList.Add(originalItem);
        }

        if (addToEnd)
        {
          foreach (var item in itemsToMove)
          {
            newList.Add(item);
          }
        }
      }
      else if (type == RepositionType.Reverse)
      {
          newList = originalList;
          int[] pos = new int[itemsToMove.Count];
          int count = 0;
          for (int i = 0; i < newList.Count; i++)
          {
              var originalItem = newList[i];
              if (itemsToMove.Contains(originalItem))
              {
                    pos[count] = i;
                    count++;
              }
              
          }

          if (itemsToMove.Count == originalList.Count)
          {
              newList = originalList.Reverse().ToList();

          }
          else
          {
              for (int i = 0; i < pos.Length / 2; i++)
              {
                  var temp = newList[pos[i]];
                  int n1 = pos[i];
                  newList[pos[i]] = newList[pos[(pos.Length - 1) - i]];
                  int n2=(pos.Length - 1) - i;
                  newList[pos[(pos.Length - 1) - i]] = temp;
              
              }
              
          }

          
          
      }

      originalList = newList;
    }
  }

}
