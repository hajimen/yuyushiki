# Yuyushiki : ANT+ Bicycle Power Viewer for Indoor Cycling

![VFD customer display playing Yuyushiki](http://kaoriha.org/yuyushiki/vfd.jpg)

Hard to read Garmin's small display? Get large VFD display and use it as a power viewer!

## Prerequisites

* Windows PC. x86 capable machine. In other words, ARM machine doesn't work.
* ANT+ USB stick. Search 'ANT+ USB' in [eBay](https://www.ebay.com/). Around $10 including shipping charge.
* VFD customer display. 2 x 20 characters, ESC/POS command mode is required. Search "VFD customer display" in [Aliexpress](https://www.aliexpress.com/). Around $60 including shipping charge.
* ANT+ power meter. No BLE support so far.
* A bike and a turbo trainer, of course :-)

## For Developer

* Written in C#, Visual Studio 2017
* If you touch ANT+ function, you'll need ANT+ license. Go to [ANT website](https://www.thisisant.com/).
* ANT+ heart rate monitor may help you.

ANT_NET.dll is slightly modified from Dynastream's. It includes ANT+ network key which is prohibited to redistribute in souce code.

## Author

Hajime NAKAZATO hajime@kaoriha.org

## License

Apache License, Version 2.0

Yuyushiki includes Dynastream's software as DLL. They are ANT+ Shared Source License.
