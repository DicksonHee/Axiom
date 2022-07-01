using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Enemy
{
    public class Enemy : MonoBehaviour
    {
        public float stateMod1, stateMod2, stateMod3;
        [SerializeField] private float detectionBar;
        [SerializeField] private float stateModifier; //changes rate based on enemy state
        [SerializeField] private float timer;

        public enum state
        {
            normal,
            alert,
            engage
        };

        public int currentState = 0;

        public float engageTimer;
        public int decreaseRate;

        [SerializeField] bool alerted = false;
        [SerializeField] bool engaged = false;

        EnemySight sight;
        float dbCheckPoint = 0;

        private void Start()
        {
            sight = GetComponent<EnemySight>();
        }

        private void FixedUpdate()
        {
            float db = detectionBar;
            detectionBar = Mathf.Clamp(db, dbCheckPoint, 10);

            //change variables depending on enemy state
            switch (currentState)
            {
                case 0:
                    stateModifier = stateMod1;
                    alerted = false;
                    engaged = false;
                    dbCheckPoint = 0;
                    break;
                case 1:
                    stateModifier = stateMod2;
                    alerted = true;
                    dbCheckPoint = 4.99f;
                    break;
                case 2:
                    stateModifier = stateMod3;
                    engaged = true;
                    dbCheckPoint = 9.99f;
                    break;
                // default:
                // stateModifier = stateMod1;
                // alerted = false; engaged = false;
                // dbCheckPoint = 0;
                // break;
            }

            //Handles detectionbar
            if (sight.inView)
            {
                detectionBar += DetectionRate(stateModifier, sight.distance) * Time.deltaTime;
            }
            else if (!engaged)
            {
                if (currentState == 1)
                {
                    return; //if player has not been spotted, but enemy is alerted, reset detectionbar elsewhere
                }
                else
                {
                    detectionBar -= decreaseRate * Time.deltaTime;
                }

            }

            if (detectionBar < 4.99f)
            {
                currentState = 0;
            }
            else if (detectionBar > 4.99f && detectionBar < 9.99f)
            {
                currentState = 1;
            }
            else if (detectionBar > 9.99f)
            {
                currentState = 2;
            }

            //timer stuff
            if (currentState == 2 && sight.inView)
            {
                timer = engageTimer;
            }
            else if (currentState == 2 && !sight.inView)
            {
                timer -= Time.deltaTime;
                if (timer < engageTimer / 2)
                {
                    currentState = 1;
                }
            }

            //timer stuff state 1
            if (currentState == 1 && !sight.inView && engaged)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    detectionBar -= decreaseRate * Time.deltaTime;
                    if (detectionBar <= 5)
                    {
                        currentState = 0;
                    }
                }
            }
            else if (currentState == 1 && !engaged)
            {
                //this means closest enemy need check player last known 
                //after checking, if nothing was found, currentstate = 0;
            }
        }

        private float DetectionRate(float _stateMod, float _distance)
        {
            float disX = 0;
            //check distance from sight
            if (_distance < sight.pointBlankRange)
            {
                disX = 10;
            }
            else if (_distance < sight.midRange)
            {
                disX = 4;
            }
            else if (_distance < sight.farRange)
            {
                disX = 2;
            }
            else
            {
                disX = 0;
            }

            return _stateMod * disX;
        }
    }
}
