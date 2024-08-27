import axios, { AxiosResponse } from "axios";
import DataHandler from "./DataHandler";

let request = {
    method: "",
    url: "",
    headers: {}, 
    data: {}
};

let response: AxiosResponse<any,any>;


function createRequest(requestType: string) {

    const user = DataHandler.getUser();

    switch(requestType) {

        case "login":
            request = {
                method: 'post',
                url: "http://10.0.2.2:5181/Mr-Pill/Login",
                headers: { "Content-Type": "application/json" }, 
                data: {
                  "PhoneNumber": user.PhoneNumber,
                }
            }; return;

        case "verifyLogin":
            request = {
                method: 'post',
                url: "http://10.0.2.2:5181/Mr-Pill/ValidateCode",
                headers: { "Content-Type": "application/json" }, 
                data: {
                  "PhoneNumber": user.PhoneNumber,
                  "Code": DataHandler.getState('validationCode'),
                }
            }; return;

        case "signup":
            request = {
                method: 'post',
                url: "http://10.0.2.2:5181/Mr-Pill/GenerateRegistrationCode",
                headers: { }, 
                data: {
                  PhoneNumber: user.PhoneNumber,
                }
            }; return;
        
        case "verifySignup":
            request = {
                method: 'post',
                url: "http://10.0.2.2:5181/Mr-Pill/Register",
                headers: { }, 
                data: {
                    FirstName: user.FirstName,
                    LastName: user.LastName,
                    PhoneNumber: user.PhoneNumber,
                    Code: DataHandler.getState('validationCode')
                }
            }; return;
        
        case "getMyPills":
            request = {
                method: 'get',
                url: "http://10.0.2.2:5194/user/all/medications",
                headers: { 
                  "Content-Type": "application/json",
                  "Authorization": "Bearer " + user.Token,
                }, 
                data: {
                  
                }
            }; return;

        case "addPill":
            request = {
                method: 'post',
                url: "http://10.0.2.2:5194/medications?medicineCabinetName=" + DataHandler.getState('medicineCabinetName'),
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
                url: "http://10.0.2.2:5194/user/cabinet",
                headers: {
                    "Authorization": "Bearer " + user.Token, 
                },
                data: {
                }
            }; return;

        case "addCabinet":
            request = {
                method: 'get',
                url: "http://10.0.2.2:5194/user/cabinet",
                headers: {
                    "Authorization": "Bearer " + user.Token, 
                },
                data: {
                }
            }; return;


        case "getMyReminders":

        case "addReminder":
            
        default:

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
                DataHandler.expireSession();
                return false;
            } else {
                console.log(response.request.status)
                return false;
            }
            
        } catch (error) {
            console.error("Error fetching data:", error);
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