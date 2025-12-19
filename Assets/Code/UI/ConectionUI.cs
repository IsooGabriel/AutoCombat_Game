using UnityEngine;

public class ConectionUI : MonoBehaviour
{
    public PortUI fromPort;
    public PortUI toPort;
    public LineRenderer lineRenderer;


    #region 関数


    public void OnClickRight()
    {
        fromPort.outputLines.Remove(lineRenderer);
        fromPort.port.outputConections.RemoveAll(conn => conn.Item1 == toPort.port.owner && conn.Item2 == toPort.port.portName);
        Destroy(gameObject);
    }


    #endregion
    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
    }
    void Update()
    {
        try
        {
            Vector3 startPos = fromPort.portPosition.position;
            Vector3 endPos = toPort.portPosition.position;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
        catch
        {
            Destroy(gameObject);
        }
    }
}