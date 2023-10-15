using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace palladinTank
{
    public class PalladinTank : PalladinsNPC, ISkills
    {
        private int _firstCircle;
        private int _secondCircle;
        private int _coolDown;

        private void Start()
        {
            _health = 100f;
            _enemyHealth = 100f;
            _firstCircle = 0;
            StartCoroutine(testSkills());
        }
        public void UseSkill()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator testSkills()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);
                var rand = UnityEngine.Random.Range(1, 3);
                if (rand == 1)
                {
                    _enemyHealth -= 1f;
                    _firstCircle += 1;
                    if (_firstCircle == 2) 
                    {
                        SetBehaviour(SetFirstSkill());
                    }
                    Debug.Log("set hit");
                }
                if (rand == 2)
                {
                    _health -= 1f;
                    Debug.Log("get hit");
                }
            }
        }

        private INPCBehaviour SetFirstSkill()
        {
            var behaviour = GetBehaviour<FirstSkill>();
            return behaviour;
        }

        protected override void InitBehaviors()
        {
            base.InitBehaviors();

            _behaviourMap[typeof(FirstSkill)] = new FirstSkill(this);
        }
    }
}
