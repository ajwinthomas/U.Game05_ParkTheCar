using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private Rigidbody playerRB;
    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;
    public WheelParticles wheelParticles;
    public float gasInput;
    public float brakeInput;
    public float steeringInput;
    public GameObject smokePrefab;

    public float motorPower;
    public float brakePower;
    private float slipAngle;
    private float speed;
    private float speedClamped; 
    public float maxSpeed;
    public AnimationCurve steeringCurve;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        InstantiateSmoke();
    }

    // Update is called once per frame
    void Update()
    {
        speed = colliders.RRWheel.rpm * colliders.RRWheel.radius * 2f * Mathf.PI / 10;
        speedClamped = Mathf.Lerp(speedClamped,speed,Time.deltaTime);
        CheckInput();
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        CheckParticles();
        UpdateWheels();
        

    }

    // This fuction is to instantiate smoke effects of the wheels.
    void InstantiateSmoke()
    {
        wheelParticles.FLWheel = Instantiate(smokePrefab,colliders.FLWheel.transform.position - Vector3.up * colliders.FLWheel.radius,Quaternion.identity , colliders.FLWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.FRWheel = Instantiate(smokePrefab, colliders.FRWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.FRWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.RRWheel = Instantiate(smokePrefab, colliders.RRWheel.transform.position - Vector3.up * colliders.RRWheel.radius, Quaternion.identity, colliders.RRWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.RLWheel = Instantiate(smokePrefab, colliders.RLWheel.transform.position - Vector3.up * colliders.RLWheel.radius, Quaternion.identity, colliders.RLWheel.transform)
            .GetComponent<ParticleSystem>();
    }


    void CheckInput()
    {
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");
        slipAngle = Vector3.Angle(transform.forward,playerRB.linearVelocity-transform.forward);

        //if you ask  me what happens here, i think we are checking that actually the car is going forward. then we apply the brake.(i guess).
        if(slipAngle < 120f)
        {
            if(gasInput < 0)
            {   
                brakeInput = Mathf.Abs(gasInput);
                gasInput = 0;
            }
        }
        else
        {
            brakeInput = 0;
        }
    }

    void ApplyBrake()
    {
        colliders.FRWheel.brakeTorque = brakeInput * brakePower * 0.7f;
        colliders.FLWheel.brakeTorque = brakeInput * brakePower * 0.7f;

        colliders.RRWheel.brakeTorque = brakeInput * brakePower * 0.3f;
        colliders.RLWheel.brakeTorque = brakeInput * brakePower * 0.3f;
    }
    // this function actually give take our input and motor power and give it to the back wheels. 
    //thats how our car moves front or back.
    void ApplyMotor()
    {

        if(speed < maxSpeed)
        {
            colliders.RRWheel.motorTorque = motorPower * gasInput;
            colliders.RLWheel.motorTorque = motorPower * steeringInput;
        }
        else
        {
            colliders.RRWheel.motorTorque = 0;
            colliders.RLWheel.motorTorque = 0;
        }
    }


    // our ApplySteering is very special compared to any other function.
    // you guessed is right. it controls the steering of the front wheels.
    //but there is a catch , according to the speed of the car. the steering angle differ.
    // which means if your car is going high speed . you can only steer a little. and vice versa.
    void ApplySteering()
    {
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speed);
        steeringAngle += Vector3.SignedAngle(transform.forward, playerRB.linearVelocity + transform.forward, Vector3.up);
        steeringAngle = Mathf.Clamp(steeringAngle, -90f, 90f);
        colliders.FLWheel.steerAngle = steeringAngle;
        colliders.FRWheel.steerAngle = steeringAngle;
    }

    //we use this function to update the rotation and position of 4 wheelcollider to 4 wheelmesh.
    private void UpdateWheels()
    {
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
    }

    void CheckParticles()
    {
        WheelHit[] wheelHits = new WheelHit[4];
        colliders.FLWheel.GetGroundHit(out wheelHits[0]);
        colliders.FRWheel.GetGroundHit(out wheelHits[1]);
        colliders.RLWheel.GetGroundHit(out wheelHits[2]);
        colliders.RRWheel.GetGroundHit(out wheelHits[3]);


        float slipAllowance = 1f;

        
        //what we are doing here is we check each wheel is slipping .
        //if it is slipping then we play the particle system of that wheel else we stop the particle system
        if (Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip)> slipAllowance)
        {
            wheelParticles.FLWheel.Play();
        }
        else
        {
            wheelParticles.FLWheel.Stop();
            
        }

        if (Mathf.Abs(wheelHits[1].sidewaysSlip) + Mathf.Abs(wheelHits[1].forwardSlip) > slipAllowance)
        {
            wheelParticles.FRWheel.Play();
        }
        else
        {
            wheelParticles.FRWheel.Stop();
        }

        if (Mathf.Abs(wheelHits[2].sidewaysSlip) + Mathf.Abs(wheelHits[2].forwardSlip) > slipAllowance)
        {
            wheelParticles.RLWheel.Play();
        }
        else
        {
            wheelParticles.RLWheel.Stop();
        }

        if (Mathf.Abs(wheelHits[3].sidewaysSlip) + Mathf.Abs(wheelHits[3].forwardSlip) > slipAllowance)
        {
            wheelParticles.RRWheel.Play();
        }
        else
        {
            wheelParticles.RRWheel.Stop();
        }

        //we need continue from here tommorrow. don't forget.
    }

    void UpdateWheel(WheelCollider wheelColl,MeshRenderer wheelMesh)
    {
        //here what we are doing is getting the rotation and position of the wheelcollider and give it to the wheelmesh;
        Quaternion quat;
        Vector3 position;
        wheelColl.GetWorldPose(out position, out quat);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = quat;
    }

    public float GetSpeedRatio()
    {
        var gas = Mathf.Clamp(gasInput, 0.5f, 1f);
        return speedClamped * gas/maxSpeed;
    }

    //custom class we create to hold the reference of 4 wheel colliders.
    [System.Serializable]
    public class WheelColliders
    {
        public WheelCollider FRWheel;
        public WheelCollider FLWheel;
        public WheelCollider RRWheel;
        public WheelCollider RLWheel;
    }

    //custom class we create to hold the reference of 4 actual wheel meshes.

    [System.Serializable]
    public class WheelMeshes
    {
        public MeshRenderer FRWheel;
        public MeshRenderer FLWheel;
        public MeshRenderer RRWheel;
        public MeshRenderer RLWheel;
    }


    //This custom class is store the partcle smoke effect for 4 wheels.
    [System.Serializable]
    public class  WheelParticles
    {
        public ParticleSystem FRWheel;
        public ParticleSystem FLWheel;
        public ParticleSystem RRWheel;
        public ParticleSystem RLWheel;
    }
}
