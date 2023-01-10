using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FoodButton : MonoBehaviour
{
    public event EventHandler OnUsed;

    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material greenDarkMaterial;
    private MeshRenderer buttonMeshRenderer;
    [SerializeField] private Transform buttonTransform;

    public UnityEvent OnClick;
    public GameObject ButtonObj;
    public GameObject FoodPrefab;
    private GameObject spawnedFood;
    public bool canUseButton = false;
    void Awake()
    {
        buttonTransform = transform.Find("Button");
        buttonMeshRenderer = buttonTransform.GetComponent<MeshRenderer>();
        canUseButton = true;
    }
    void Start()
    {
        ResetButton();
    }

    public bool CanUseButton() => canUseButton;

    public void UseButton()
    {
        if (canUseButton)
        {
            buttonMeshRenderer.material = greenDarkMaterial;
            buttonTransform.localPosition = new Vector3(0, 0.025f, 0);
            canUseButton = false;
            SpawnFood();
            OnUsed?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ResetButton()
    {
        if (spawnedFood != null)
            Destroy(spawnedFood);
        buttonMeshRenderer.material = greenMaterial;
        buttonTransform.localPosition = new Vector3(0, 0.1f, 0);

        transform.localPosition = new Vector3(
            transform.localPosition.x, transform.localPosition.y, UnityEngine.Random.Range(-2f, 2f)
            );

        canUseButton = true;
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space) && !canUseButton)
        // {
        //     canUseButton = true;
        //     OnClick?.Invoke();
        //     Vector3 pos = ButtonObj.transform.localPosition;
        //     ButtonObj.transform.localPosition = new Vector3(pos.x, 0.025f, pos.z);
        //     SpawnFood();
        // }

    }

    public bool HasFoodSpawned()
    {
        return spawnedFood != null;
    }

    void SpawnFood()
    {
        Vector3 randPos = new Vector3(UnityEngine.Random.Range(-7f, 7f), 0.5f, UnityEngine.Random.Range(-2f, 2f));

        spawnedFood = Instantiate(FoodPrefab, randPos, Quaternion.identity);
        spawnedFood.transform.SetParent(transform.parent, false);
    }

    public Vector3 GetLastFoodTransform()
    {
        return spawnedFood.transform.localPosition;
    }
}
