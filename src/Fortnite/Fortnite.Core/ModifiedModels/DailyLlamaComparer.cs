using System;
using System.Collections.Generic;

namespace Fortnite.Core.ModifiedModels
{
    internal class DailyLlamaComparer : IEqualityComparer<DailyLlama>
    {
        public bool Equals(DailyLlama x, DailyLlama y)
        {
            if (string.Equals($"{x.Title}:{x.Price}", $"{y.Title}:{y.Price}", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(DailyLlama obj)
        {
            if (obj.Title == null)
            {
                var ix1 = obj.DevName.IndexOf(" x ") + 3;
                var ix2 = obj.DevName.IndexOf(" for ");
                obj.Title = obj.DevName.Substring(ix1, ix2 - ix1);
                obj.Description = obj.Title;
                if (obj.Amount == -1)
                {
                    obj.Amount = 0;
                }
            }
            return obj.Title.GetHashCode();
        }
    }
}