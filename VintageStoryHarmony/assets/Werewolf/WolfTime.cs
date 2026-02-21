using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;

namespace WereWolf.assets.Werewolf
{
    internal class WolfTime
    {
        public static bool isNight(Entity entity)
        {
            var time = entity?.World?.Calendar.HourOfDay;

            if (time >= 18 || time <= 6)
            {
                return true;
            }
            else return false;
           
           
        }

      
            }
        }


    
