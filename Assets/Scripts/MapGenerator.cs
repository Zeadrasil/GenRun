using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] start5, start7, start9, start11;
    private GameObject[][] mapPieces = new GameObject[4][];
    private GameObject current = null;
    [SerializeField] VoidEvent startEvent = null;
    [SerializeField] VoidEvent dieEvent = null;
    [SerializeField] VoidEvent clearEvent = null;
    void Start()
    {
        startEvent?.Subscribe(startGame);
        dieEvent?.Subscribe(clear);
        clearEvent?.Subscribe(generate);
        mapPieces[0] = start5;
        mapPieces[1] = start7;
        mapPieces[2] = start9;
        mapPieces[3] = start11;
    }
    private void startGame()
    {
        current = selectMapPiece(7);
        current = Instantiate(current, new Vector3(32, 4, 0), Quaternion.identity);
        MapPiece pieceData = current.GetComponent<MapPiece>();
        current = Instantiate(selectMapPiece(pieceData.endSize), pieceData.nextStart.position, Quaternion.identity);
        pieceData.next = current.GetComponent<MapPiece>();
        pieceData.next.previous = pieceData;
        current = pieceData.gameObject;
    }
    private void generate()
    {
        MapPiece pieceData = current.GetComponent<MapPiece>();
        while(pieceData.next != null)
        {
            pieceData = pieceData.next;
        }
        GameObject obj = Instantiate(selectMapPiece(pieceData.endSize), pieceData.nextStart.position, Quaternion.identity);
        pieceData.next = obj.GetComponent<MapPiece>();
        pieceData.next.previous = pieceData;
        pieceData = current.GetComponent<MapPiece>();
        if(pieceData.previous != null)
        {
            if(pieceData.previous.previous != null)
            {
                pieceData.previous.previous.destroy();
            }
        }
        current = pieceData.next.gameObject;
    }

    private GameObject selectMapPiece(int size)
    {
        switch(size)
        {
            case 5:
                return start5[Random.Range(0, start5.Length)];
            case 7:
                return start7[Random.Range(0, start7.Length)];
            case 9:
                return start9[Random.Range(0, start9.Length)];
            case 11:
                return start11[Random.Range(0, start11.Length)];

        }
        return null;
    }

    private void clear()
    {
        if(current != null)
        { 
            current.GetComponent<MapPiece>().clear();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
