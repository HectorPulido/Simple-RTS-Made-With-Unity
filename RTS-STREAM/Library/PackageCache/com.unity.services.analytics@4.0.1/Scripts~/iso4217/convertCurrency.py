import json
import xmltodict

with open("iso4217.xml") as xml_file:
    data_dict = xmltodict.parse(xml_file.read())
    xml_file.close()

    array_of_currencies = data_dict["ISO_4217"]["CcyTbl"]["CcyNtry"]

    formatted_currency_dict = {}
    for currency_dict in array_of_currencies:
        if ("Ccy" in currency_dict) and ("CcyMnrUnts" in currency_dict):
            currency_code = currency_dict["Ccy"]
            currency_minor_units = currency_dict["CcyMnrUnts"]
            if currency_minor_units == "N.A.":
                print("No minor units data available for " + currency_dict["CtryNm"])
            else:
                formatted_currency_dict[currency_code] = int(currency_minor_units)
        elif ("CtryNm" in currency_dict):
            print("No currency data available for " + currency_dict["CtryNm"])
        else:
            print("Currency in file with no data at all, weird.")

    json_data = json.dumps(formatted_currency_dict)

    with open("../../Runtime/Resources/iso4217.json", "w") as json_file:
        json_file.write(json_data)
        json_file.close()

