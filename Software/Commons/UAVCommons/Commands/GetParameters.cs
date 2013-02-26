using System;
using System.Collections.Generic;

using System.Text;

namespace UAVCommons.Commands
{
    /// <summary>
    /// This Command fetches all the Data in uavdata from the Remote Site and adds /replaces it in the Local uavdata
    /// Can be used to sync Max Min and updateRate
    /// Will be called Automatically on Connect 
    /// </summary>
    [Serializable]
    public class GetParameters : BaseCommand
    {
        public List<UAVSingleParameter> uavData = new List<UAVSingleParameter>();

        public override void Send(UAVBase core)
        {
            base.Send(core);
        }

        public override void RemoteExecute(UAVBase core)
        {
            base.RemoteExecute(core);
            //   remoteuavData = core.uavData;
            uavData = HierachyItem.NormaliseUavData(core);
			Console.WriteLine("Anzahl der Punkte die gesendet werden:"+uavData.Count);
        }

        

     

        public override void ProcessResults(UAVBase core)
        {
            base.ProcessResults(core);
            try
            {
                foreach (UAVSingleParameter param in this.uavData)
                {
                    if (param.Value == null) param.Value = "";
                    UAVSingleParameter newparam = core.uavData.SilentUpdate(param.Name, param.Value, true);
                    if (newparam != null)
                    {
                       if (newparam.Parent is UAVStructure) ((UAVStructure)newparam.Parent).ValueChanged+=new UAVStructure.ValueChangedHandler(core.uavData.Value_ValueChanged);
                        newparam.Min = param.Min;
                        newparam.Max = param.Max;
                        newparam.updateRate = param.updateRate;
                        newparam.Parent = core.uavData.GetFromPath(param.Name.Substring(0,param.Name.LastIndexOf("\\")));
                        if (newparam.Parent == null) newparam.Parent = core;
                        if (newparam.Parent.Name == newparam.Name) newparam.Parent = core;
                    }
                    else
                    {
                        core.uavData.Add(param.Name, (UAVParameter)param.Clone());
                    }
                }
            }
            catch (Exception ex)
            {

            }
            core.initialised = true;
        }

          }
}
