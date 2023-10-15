using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace palladinTank
{
    public class FirstSkill : INPCBehaviour
    {
        private PalladinTank _palladinTank;

        public void Enter()
        {
            Debug.Log("Enter FirstSkill");
            throw new System.NotImplementedException();
        }
        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }


        public FirstSkill(PalladinTank palladin) 
        { 
            _palladinTank = palladin;
        }
    }

}
