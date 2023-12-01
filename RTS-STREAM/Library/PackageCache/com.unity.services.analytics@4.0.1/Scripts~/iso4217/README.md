# Currency Conversion Script

This script is used for converting the ISO currency code data from XML into JSON, to make it easier to work with in our SDK. It also removes some of the unneeded data, so that we keep the size of our SDK to a minimum.

## Requirements

* Python 3

## Using the Script

Instructions for macOS (windows neither tested nor supported)

* Download the latest ISO 4217 XML spec, and move it into the `Scripts~/iso4217` folder, and rename it to `iso4217.xml`
* Make sure you are in the `Scripts~/iso4217` folder in your terminal
* Run `pip3 install -r requirements.txt` to install script dependencies
* Run `python3 convertCurrency.py` to update the JSON resource file.


