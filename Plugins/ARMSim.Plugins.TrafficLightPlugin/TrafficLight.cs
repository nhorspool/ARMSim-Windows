using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using ARMPluginInterfaces;

namespace ARMSim.Plugins.TrafficLightPlugin
{
    public class TrafficLight : IARMPlugin
    {
        private IARMHost mHost;
        private TrafficIntersectionControl mTrafficIntersectionControl;

        public string PluginName { get { return "TrafficLight"; } }
        public string PluginDescription { get { return "Simulated Traffic Intersection"; } }


        public void Init(IARMHost iHost)
        {
            mHost = iHost;

            //set the load handler so we get called when all the plugins are loaded
            mHost.Load += onLoad;
        }

        //called after all plugins have had their init called and are all loaded
        public void onLoad(object sender, EventArgs e)
        {
            mHost.Restart += onRestart;

            Panel panel = mHost.RequestPanel(this.PluginName);
            mTrafficIntersectionControl = new TrafficIntersectionControl();
            panel.Controls.Add(mTrafficIntersectionControl);

            //request that we get all SWI calls for the range 0x0100 - 0x0x1ff
            mHost.RequestOpCodeRange(0x0f000100, 0x0fffff00, this.onExecute);

        }

        public void onRestart(object sender, EventArgs e)
        {
            mTrafficIntersectionControl.Reset();
        }

        //called when our requested opcode(s) have been fetched and need executing
        public uint onExecute(uint opcode)
        {
            switch (opcode & 0x000001ff)
            {
                //set traffic light (r0 = light, 0 - main, 1 - side:r1 = state, 0-black,1-red,2-green,3-yellow)
                case 0x100:
                    {
                        UIControls.TrafficLight trafficLight;
                        int light = (int)mHost.getReg(0);
                        if (light == 0)
                            trafficLight = mTrafficIntersectionControl.MainTrafficLight;
                        else if (light == 1)
                            trafficLight = mTrafficIntersectionControl.SideTrafficLight;
                        else
                        {
                            mHost.OutputConsoleString("Bad traffic light code specified:{0}", light);
                            return 1;
                        }

                        int code = (int)mHost.getReg(1);
                        if (code < 0 || code > 3)
                        {
                            mHost.OutputConsoleString("Bad traffic light state specified:{0}", code);
                            return 1;
                        }
                        trafficLight.State = (UIControls.TrafficLight.TrafficLightStates)code;
                    }
                    break;

                //check xwalk button (r0 = button to check 0-1)
                //returns:r0 , 0 not pushed, 1 pushed
                case 0x101:
                    {
                        int button = (int)mHost.getReg(0);
                        bool pressed = false;
                        if (button == 0)
                        {
                            pressed = mTrafficIntersectionControl.GetMainButtonPressed();
                        }
                        else if (button == 1)
                        {
                            pressed = mTrafficIntersectionControl.GetSideButtonPressed();
                        }
                        else
                        {
                            mHost.OutputConsoleString("Bad traffic light button specified:{0}", button);
                            return 1;
                        }
                        mHost.setReg((uint)0, (uint)(pressed ? 1 : 0));
                    }
                    break;
                //set xwalk sign (r0 - 0 walk, 1 dont walk, 2 hurry)
                case 0x102:
                    {
                        UIControls.WalkSignal walkSignal;
                        int walk = (int)mHost.getReg(0);
                        if (walk == 0)
                            walkSignal = mTrafficIntersectionControl.MainWalkSignal;
                        else if (walk == 1)
                            walkSignal = mTrafficIntersectionControl.SideWalkSignal;
                        else
                        {
                            mHost.OutputConsoleString("Bad walk code specified:{0}", walk);
                            return 1;
                        }

                        int code = (int)mHost.getReg(1);
                        if (code < 0 || code > 2)
                        {
                            mHost.OutputConsoleString("Bad walk code specified:{0}", code);
                            return 1;
                        }
                        walkSignal.State = (UIControls.WalkSignal.XWalkStates)code;
                    } break;

                default: break;
            }

            //return true indicating opcode was processed
            return 3;
        }//onExecute



    }
}
