from cvzone.HandTrackingModule import HandDetector
import cv2
import time
import socket
import math


def coef(x0, y0, z0, x9, y9, z9):
    x0 += 0.00000000000000000000001
    y0 += 0.00000000000000000000001
    z0 += 0.00000000000000000000001
    length = math.sqrt((x9 - x0) ** 2 + (y9 - y0) ** 2 + (z9 - z0) ** 2)
    coeff = length / 800
    return coeff



cap = cv2.VideoCapture(0)
cap.set(0, 0)
cap.set(0, 0)
success, img = cap.read()
h, w, _ = img.shape
detector = HandDetector(detectionCon=0.8, maxHands=2)
host, port = "127.0.0.1", 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((host, port))


while True:
    print('go')
    success, img = cap.read()
    hands, img = detector.findHands(img)

    if hands:
        # Hand 1
        hand = hands[0]
        lmList = hand["lmList"]
        coeff = coef(lmList[0][0], lmList[0][1], lmList[0][2], lmList[9][0], lmList[9][1], lmList[9][2])
        coords = []
        for i in lmList:
            for j in range(3):
                if j == 1:
                    coords.append(str(int(h - i[j])))
                else:
                    coords.append(str(int(i[j])))
        coords = list(map(lambda x: str(int(int(x)/coeff)), coords))
        print(coords)
        coords = " ".join(coords)
        time.sleep(0.1)
        sock.sendall(coords.encode("UTF-8"))
        receivedData = sock.recv(1024).decode("UTF-8")
        print(receivedData)
    cv2.imshow("Image", img)
    cv2.waitKey(1)
