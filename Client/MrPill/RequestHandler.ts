import axios, { AxiosResponse } from "axios";
import DataHandler from "./DataHandler";


// change if need
const BASE_URL = "http://123.123.123.123:"
const SERVER_AND_CLIENT_ON_SAME_MACHINE = true;

// do not change
const BASE_URL_LOCAL = "http://10.0.2.2:" // android emulator and server running on same machine
const URL = SERVER_AND_CLIENT_ON_SAME_MACHINE ? BASE_URL_LOCAL : BASE_URL;

let request = {
    method: "",
    url: "",
    headers: {}, 
    data: {}
};

let response: AxiosResponse<any, any>;

function createRequest(requestType: string) {

    const user = DataHandler.getUser();

    switch (requestType) {

        case "login":
            request = {
                method: 'post',
                url: URL + "5181/Mr-Pill/Login",
                headers: { "Content-Type": "application/json" }, 
                data: {
                    "PhoneNumber": user.PhoneNumber,
                }
            }; return;

        case "verifyLogin":
            request = {
                method: 'post',
                url: URL + "5181/Mr-Pill/ValidateCode",
                headers: { "Content-Type": "application/json" }, 
                data: {
                    "PhoneNumber": user.PhoneNumber,
                    "Code": DataHandler.getState('validationCode'),
                }
            }; return;

        case "signup":
            request = {
                method: 'post',
                url: URL + "5181/Mr-Pill/GenerateRegistrationCode",
                headers: { }, 
                data: {
                    PhoneNumber: user.PhoneNumber,
                }
            }; return;
        
        case "verifySignup":
            request = {
                method: 'post',
                url: URL + "5181/Mr-Pill/Register",
                headers: { }, 
                data: {
                    FirstName: user.FirstName,
                    LastName: user.LastName,
                    PhoneNumber: user.PhoneNumber,
                    Code: DataHandler.getState('validationCode')
                }
            }; return;
        
        case "getAllPills":
            request = {
                method: 'get',
                url: URL + "5194/user/all/medications",
                headers: { 
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + user.Token,
                }, 
                data: {}
            }; return;

        case "getPills":
            request = {
                method: 'get',
                url: URL + "5194/user/medications?medicineCabinetName=" + DataHandler.getState("medicineCabinetName"),
                headers: { 
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + user.Token,
                }, 
                data: {}
            }; return;

        case "addPill":

            request = {
                method: 'post',
                url: URL + "5194/medications?medicineCabinetName=" + DataHandler.getState('medicineCabinetName'),
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {
                    MedicationBarcode: DataHandler.getState('medicationBarcode'),
                    Privacy: false
                }
            }; return;

        case "getMyCabinets":
            request = {
                method: 'get',
                url: URL + "5194/user/cabinet",
                headers: {
                    "Authorization": "Bearer " + user.Token, 
                },
                data: {}
            }; return;

        case "addCabinet":
            request = {
                method: 'post',
                url: URL + "5194/medicine-cabinet?Name=" + DataHandler.getState("medicineCabinetName"),
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {}
            }; return;

        case "getMyReminders":
            request = {
                method: 'get',
                url: URL + "5194/Reminders",
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {}
            }; return;

        case "addReminder":
            request = {
                method: 'post',
                url: URL + "5194/SetReminder",
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: DataHandler.getReminder(),
            
            }; return;

        default:
            console.error("Invalid request type");
    }
}

export default {
    async sendRequest(requestType: string) {

        try {
            axios.defaults.validateStatus = function () {
                return true;
            };

            createRequest(requestType);

            response = await axios(request);
            
            if (response.request.status == 200) {
                return true;
            } else if (response.request.status == 401) {
                if (requestType != "verifySignup" && requestType != "verifyLogin") {
                    DataHandler.expireSession();
                }
                return false;
            } else {
                console.log(response.request.status);
                return false;
            }
            
        } catch (error) {
            console.error("Error fetching data:", error);
            response = {} as AxiosResponse<any,any>
            return false;
        }
    },

    getRequest() {
        return request;
    },

    getResponse() {
        return response;
    }
};
