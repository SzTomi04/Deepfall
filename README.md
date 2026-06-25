# Deepfall 🎮

Egy izgalmas, akció-orientált 2D platformer játék, Unity motorban fejlesztve. A játék fókusza a precíz mozgáson, a falmászáson (wall-jump) és az interaktív pályaelemeken van.

## ✨ Főbb funkciók és mechanikák

* **Precíz platformer irányítás:** Sima mozgás, ugrás, és egyedi falra tapadós/falról elrugaszkodós (wall-jump) mechanika.
* **Optimalizált 2D Fizika:** Zökkenőmentes mozgás a sarkoknál, Composite Collider technológiával kisimított Tilemap környezet.
* **Interaktív világ:** Kulcs-ajtó logikára épülő pályaváltási rendszer.
* **Dinamikus UI és Életerő:** Játékoshoz tapadó, valós időben frissülő Health Bar, sebzés- és halál-animációkkal.
* **Modern bemenet-kezelés:** A játék a Unity új Input System-jét (New Input System) használja a reszponzív irányítás érdekében.

## ⌨️ Irányítás

A játék teljes mértékben támogatja a billentyűzetes irányítást:

| Gomb | Akció |
| :--- | :--- |
| **A / D** vagy **Nyilak** | Mozgás balra és jobbra |
| **Space** | Ugrás / Elrugaszkodás a falról |
| **E** | Interakció (pl. Zárt ajtók kinyitása, ha megvan a kulcs) |
| **R** | Játék újraindítása (Game Over képernyőn) |

## 🚀 Telepítés és Futtatás

Ha csak játszani szeretnél a játékkal:

1. Navigálj a GitHub oldal **Releases** fülére, és töltsd le a legújabb `.zip` fájlt.
2. Csomagold ki a letöltött archívumot egy tetszőleges mappába a gépeden.
3. Indítsd el a `Deepfall.exe` fájlt.  
*(Figyelem: Az `.exe` fájlt soha ne mozgasd el a mellette lévő `_Data` mappa mellől, különben a játék nem indul el!)*

## 🛠️ Fejlesztői környezet (Tech Stack)

* **Játékmotor:** Unity (2D Core)
* **Programozási nyelv:** C#
* **Csomagok:** Unity New Input System, 2D Tilemap Editor

## 🤝 Hozzájárulás (Contributing)

Mivel ez egy személyes fejlesztésű projekt, a közvetlen hozzájárulások (Pull Requestek) jelenleg korlátozottak, de ha találsz egy hibát (bugot), nyugodtan nyiss neki egy új **Issue**-t a GitHubon!

---
*Készült C# nyelven, sok kávéval és Unity-vel.*