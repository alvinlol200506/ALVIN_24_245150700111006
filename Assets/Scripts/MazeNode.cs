using UnityEngine;

public enum NodeState
{
    Available,
    Current,
    Completed
}

public class mazeNode : MonoBehaviour
{
    [SerializeField] public GameObject[] walls;
    [SerializeField] public MeshRenderer floor;

    public void RemoveWall(int wallToRemove)
    {
        walls[wallToRemove].gameObject.SetActive(false);
    }

    public void SetState(NodeState state)
    {
        switch (state)
        {
            case NodeState.Available:
                floor.material.color = Color.blue;
                break;
            case NodeState.Current:
                floor.material.color = Color.red;
                break;
            case NodeState.Completed:
                floor.material.color = Color.white;
                break;
        }
    }

}

