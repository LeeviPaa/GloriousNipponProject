namespace VRTK.Examples
{
    using UnityEngine;
    using System.Collections.Generic;

    public class RendererOffOnDash : MonoBehaviour
    {
        private bool wasSwitchedOff = false;
        private List<VRTK_DashTeleport> dashTeleporters = new List<VRTK_DashTeleport>();

        private void OnEnable()
        {
            foreach (var teleporter in VRTK_ObjectCache.registeredTeleporters)
            {
                var dashTeleporter = teleporter.GetComponent<VRTK_DashTeleport>();
                if (dashTeleporter)
                {
                    dashTeleporters.Add(dashTeleporter);
                }
            }

            foreach (var dashTeleport in dashTeleporters)
            {
                dashTeleport.WillDashThruObjects += new BlinkDashEventHandler(RendererOff);
                dashTeleport.DashedThruObjects += new BlinkDashEventHandler(RendererOn);
            }
        }

        private void OnDisable()
        {
            foreach (var dashTeleport in dashTeleporters)
            {
                dashTeleport.WillDashThruObjects -= new BlinkDashEventHandler(RendererOff);
                dashTeleport.DashedThruObjects -= new BlinkDashEventHandler(RendererOn);
            }
        }

        private void RendererOff(object sender, BlinkDashEventArgs e)
        {
            GameObject go = this.transform.gameObject;
            foreach (RaycastHit hit in e.hits)
            {
                if (hit.collider.gameObject == go)
                {
                    SwitchRenderer(go, false);
                    break;
                }
            }
        }

        private void RendererOn(object sender, BlinkDashEventArgs e)
        {
            GameObject go = this.transform.gameObject;
            if (wasSwitchedOff)
            {
                SwitchRenderer(go, true);
            }
        }

        private void SwitchRenderer(GameObject go, bool enable)
        {
            go.GetComponent<Renderer>().enabled = enable;
            wasSwitchedOff = !enable;
        }
    }
}