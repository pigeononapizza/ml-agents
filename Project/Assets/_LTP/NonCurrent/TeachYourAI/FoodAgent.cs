using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class FoodAgent : Agent
{
    //FoodAgent will contain 3 branches
    //1. X Movement : 3 actions (nothing, left, right)
    //2. Z Movement : 3 actions (nothing, forward, backward)
    //3. Button Press : 2 actions (nothing, press)
    public event EventHandler OnAteFood;
    public event EventHandler OnEpisodeBeginEvent;

    [SerializeField] private FoodButton foodButton;

    private Rigidbody agentRb;

    new private void Awake()
    {
        agentRb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // var randX = UnityEngine.Random.Range(-2.5f, 2.5f);
        var randZ = UnityEngine.Random.Range(-2.0f, 2.0f);
        transform.localPosition = new Vector3(0, 0, randZ);
        OnEpisodeBeginEvent?.Invoke(this, EventArgs.Empty);
        foodButton.ResetButton();

        agentRb.velocity = Vector3.zero;
        agentRb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(foodButton.CanUseButton() ? 1 : 0);

        Vector3 dirToFoodButton = (foodButton.transform.localPosition - transform.localPosition);
        sensor.AddObservation(dirToFoodButton.x);
        sensor.AddObservation(dirToFoodButton.z);

        sensor.AddObservation(foodButton.HasFoodSpawned() ? 1 : 0);

        if (foodButton.HasFoodSpawned())
        {
            Vector3 dirToFood = (foodButton.GetLastFoodTransform() - transform.localPosition).normalized;
            sensor.AddObservation(dirToFood.x);
            sensor.AddObservation(dirToFood.z);
        }
        else
        {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveX = actions.DiscreteActions[0]; // 0= Dont Move, 1= Move Left, 2= Move Right
        int moveZ = actions.DiscreteActions[1];

        Vector3 addForce = Vector3.zero;

        switch (moveX)
        {
            case 0: addForce.x = 0; break;
            case 1: addForce.x = -1; break;
            case 2: addForce.x = 1; break;
        }

        switch (moveZ)
        {
            case 0: addForce.z = 0; break;
            case 1: addForce.z = -1; break;
            case 2: addForce.z = 1; break;
        }

        float moveSpeed = 5f;
        agentRb.velocity = addForce * moveSpeed;

        bool isUseButtonDown = actions.DiscreteActions[2] == 1;
        if (isUseButtonDown)
        {
            Debug.Log("Use ButtonDown");
            Collider[] colliderArray = Physics.OverlapBox(transform.position, Vector3.one * .5f);
            foreach (var collider in colliderArray)
            {
                if (collider.TryGetComponent<FoodButton>(out FoodButton foodButton))
                {
                    if (foodButton.CanUseButton())
                    {
                        foodButton.UseButton();
                        AddReward(1f);
                        break;
                    }
                }
            }
        }

        // AddReward(-1f/ MaxStep);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        switch (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")))
        {
            case -1: discreteActions[0] = 1; break;
            case 0: discreteActions[0] = 0; break;
            case 1: discreteActions[0] = 2; break;
        }
        switch (Mathf.RoundToInt(Input.GetAxisRaw("Vertical")))
        {
            case -1: discreteActions[1] = 1; break;
            case 0: discreteActions[1] = 0; break;
            case 1: discreteActions[1] = 2; break;
        }
        discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Food>(out Food food))
        {
            AddReward(1f);
            Destroy(food.gameObject);
            OnAteFood?.Invoke(this, EventArgs.Empty);

            EndEpisode();
        }
    }
}
