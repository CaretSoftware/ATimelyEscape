using CallbackSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace CallbackSystem {
    [RequireComponent(typeof(CubePush))]
    [RequireComponent(typeof(TimeTravelObject))]
    [RequireComponent(typeof(MeshRenderer))]
    public class CubeCharge : MonoBehaviour
    {
        private static readonly int chargeReductionAfterTimeJump = 1;

        [Tooltip("Charge reducese with 1 every forward jump in time. At 0 cube can no longer be puched")]
        public int charge = 0;
        public CubeCharge pastCubeCharge;

        [SerializeField] private Material onMaterial;
        [SerializeField] private Material offMaterial;

        private CubePush cubePush;
        private ChargeChangedEvent chargeEvent;
        private TimeTravelObject timeTravelObject;
        private MeshRenderer meshRenderer;
        

        private void Start()
        {
            cubePush = GetComponent<CubePush>();
            timeTravelObject = GetComponent<TimeTravelObject>();
            meshRenderer = GetComponent<MeshRenderer>();
            if (charge > 0)
            {
                cubePush.SetPushable(true);

            } else cubePush.SetPushable(false);
            chargeEvent = new(timeTravelObject);

            ChargeChangedEvent.AddListener<ChargeChangedEvent>(PastChargeChange);
        }

        public void Charging(int charge)
        {
            if (this.charge < charge)
            {
                this.charge = charge;
                chargeEvent.Invoke();
            }
        }

        private void PastChargeChange(ChargeChangedEvent chargeChangedEvent)
        {
            if (chargeChangedEvent.changedObject.Equals(timeTravelObject)){
                if (pastCubeCharge == null)
                {
                    pastCubeCharge = timeTravelObject.pastSelf.GetComponent<CubeCharge>();
                }
                if (pastCubeCharge.pastCubeCharge != null && charge < pastCubeCharge.pastCubeCharge.charge - (2 * chargeReductionAfterTimeJump))
                {
                    charge = pastCubeCharge.pastCubeCharge.charge - (2 * chargeReductionAfterTimeJump);
                }
                if (pastCubeCharge != null && charge < pastCubeCharge.charge - chargeReductionAfterTimeJump)
                {
                    charge = pastCubeCharge.charge - chargeReductionAfterTimeJump;
                }

                if(charge > 0)
                {
                    SetMaterial(onMaterial);
                }
                else
                {
                    SetMaterial(offMaterial);
                }
            }
        }

        private void SetMaterial(Material material)
        {
            meshRenderer.material = material;
        }

    } 
}
