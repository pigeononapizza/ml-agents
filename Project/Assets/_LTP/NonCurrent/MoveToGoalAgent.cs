using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] GameObject goal;
    [SerializeField] MeshRenderer floorRenderer;
    [SerializeField] Material winMaterial;
    [SerializeField] Material loseMaterial;

    void Start()
    {
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(goal.transform.position);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Debug.Log($"Discrete{actions.DiscreteActions[0]}");
        // Debug.Log($"Continuous{actions.ContinuousActions[0]}");

        // // Get the action from the action buffer
        // var action = actionBuffers.DiscreteActions[0];

        // // Move the agent
        // switch (action)
        // {
        //     case 0:
        //         transform.Translate(0, 0, 0.1f);
        //         break;
        //     case 1:
        //         transform.Translate(0, 0, -0.1f);
        //         break;
        //     case 2:
        //         transform.Translate(0.1f, 0, 0);
        //         break;
        //     case 3:
        //         transform.Translate(-0.1f, 0, 0);
        //         break;
        // }

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 3f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goalComponent))
        {
            SetReward(1f);
            floorRenderer.material = winMaterial;
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            float distanceToGoal = Vector3.Distance(transform.localPosition, goal.transform.localPosition);
            SetReward(-1f * distanceToGoal);
            floorRenderer.material = loseMaterial;
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
        // goal.transform.localPosition = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-2f, 2f));
    }


}
