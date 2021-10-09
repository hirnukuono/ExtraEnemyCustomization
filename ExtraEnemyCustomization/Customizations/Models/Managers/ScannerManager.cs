using Agents;
using EECustom.Utils;
using Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.Models.Managers
{
    public class ScannerManager : MonoBehaviour
    {
        public EnemyAgent _Agent;
        public Color _DefaultColor;
        public Color _WakeupColor;
        public Color _DetectionColor;
        public Color _HeartbeatColor;

        public bool _UsingDetectionColor = false;

        public ScannerManager(IntPtr ptr) : base(ptr)
        {
            
        }

        internal void Start()
        {

        }

        internal void Update()
        {
            switch (_Agent.AI.Mode)
            {
                case AgentMode.Hibernate:
                    if (_Agent.IsHibernationDetecting)
                    {
                        if (_Agent.Locomotion.Hibernate.m_heartbeatActive)
                        {
                            _Agent.ScannerColor = _HeartbeatColor;
                        }
                        else
                        {
                            _Agent.ScannerColor = _DetectionColor;
                        }
                    }
                    else
                    {
                        _Agent.ScannerColor = _DefaultColor;
                    }
                    break;

                case AgentMode.Agressive:
                    _Agent.ScannerColor = _WakeupColor;
                    break;

                case AgentMode.Scout:
                    var detection = _Agent.Locomotion.ScoutDetection.m_antennaDetection;
                    if (detection == null)
                        break;

                    if (detection.m_wantsToHaveTendrils)
                    {
                        _Agent.ScannerColor = _HeartbeatColor;
                    }
                    else
                    {
                        _Agent.ScannerColor = _DetectionColor;
                    }
                    break;
            }
        }
    }
}
