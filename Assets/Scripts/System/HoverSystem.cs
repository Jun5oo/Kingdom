using UnityEngine;

public class HoverSystem : MonoBehaviour
{
     private IHoverable currentHoverable = null;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50f))
        {
            IHoverable hoverable = null;

            if (hit.transform.gameObject.TryGetComponent<IHoverable>(out hoverable))
            {
                if(hoverable != currentHoverable)
                {
                    ExitHover();
                    EnterHover(hoverable); 
                }
            }
        }

        else
        {
            if(currentHoverable != null)
                ExitHover();
        }
    }

    void EnterHover(IHoverable hoverable)
    {
        currentHoverable = hoverable;
        currentHoverable.OnHover();
    }
    void ExitHover()
    {
        currentHoverable?.OffHover();
        currentHoverable = null; 
    }
}
