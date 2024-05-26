using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    // ���������� ��� ���������������� �� ����. �� �����
    [SerializeField]
    float sensX;
    [SerializeField]
    float sensY;
    // ���������� ��� �����������, ������ � ���� ������ ����� ���������
    [SerializeField]
    Transform orientation;
    // ���������� �������� ��������
    float xRotation;
    float yRotation;
    // ��������� ��� ���������� ����� �� ��� Y
    [SerializeField]
    Transform body;
    // ���������� ��� ����������� ������ �� ��� X
    [SerializeField]
    Transform head;

    private void Start()
    {
        // ��������� ������ � ������ ��� ���������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // ���������� ���������� �����.
        // �� �������� ��� ������������� �� ������ � ��������� �� ����������������   
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        // �������� �������� ���� � ������������ �������� �� -90 �� 90
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        // ������������� �������� ��� ������ �������� ����������� ��� ������ Quaternion
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        body.rotation = Quaternion.Euler(0, yRotation, 0);
        head.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
