import axios, { AxiosResponse } from "axios";
import DataHandler from "./DataHandler";

// change if need 
const BASE_URL = "http://20.217.66.65:"
const SERVER_AND_CLIENT_ON_SAME_MACHINE = false;
const COOLDOWN_PERIOD = 1500; // time in MS user has to wait between requests of the same type.
const DEFAULT_SLEEP_TIME = 1000;
// currently unused
// const COOLDOWN_MULTIPLIER = 3; // if the user sends consequtive requests of DIFFERENT types, they can send them COOLDOWN_MULTIPLIER times faster.

// do not change
const BASE_URL_LOCAL = "http://10.0.2.2:" // android emulator and server running on same machine
const URL = SERVER_AND_CLIENT_ON_SAME_MACHINE ? BASE_URL_LOCAL : BASE_URL;

let request = {
    method: "",
    url: "",
    headers: {}, 
    data: {}
};

let lastRequestTime: number = 0;
let lastRequestType: string = "NOSUCHREQUEST";

let response: AxiosResponse<any, any>;
let parsedResponse: any;
let alreadyAwaitingResponse: boolean;

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
                    Privacy: DataHandler.getFlag("privatePill"),
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
                url: URL + "5195/Reminders",
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {}
            }; return;

        case "getMyRemindersToday":
            request = {
                method: 'get',
                url: URL + "5195/Reminders/today",
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {}
            }; return;

        case "addReminder":
            request = {
                method: 'post',
                url: URL + "5195/SetReminder",
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: DataHandler.get('reminder'),
            
            }; return;

        case "approveReminder":
            request = {
                method: 'put',
                url: URL + "5195/ApproveReminder?Id=" + DataHandler.getState("reminderId") + "&Approve=" + DataHandler.getFlag("approveReminder"),
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {},
            
            }; return;

        case "addPersonToCabinet":
            request = {
                method: 'post',
                url: `${URL}5181/Mr-Pill/joined-new-house?targetPhoneNumber=${DataHandler.getState("targetPhoneNumber")}&medicineCabinetName=${DataHandler.getState("medicineCabinetName")}`,
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {},
            }; return;

        case "getCabinetMembers":
            request = {
                method: 'get',
                url: URL + "5194/users/cabinet?cabinetId=" + DataHandler.get('cabinet').id,
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {},
            }; return;

        case "getNotifications":
            request = {
                method: 'get',
                url: URL + "5181/get-notifications",
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {},
            }; return;

        case "respondToJoinCabinetRequest":
            request = {
                method: 'put',
                url: URL + "5181/handle-notification?requestId=" + DataHandler.get('notification').id + "&approve=" + DataHandler.getFlag('userResponse'),
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {},
            }; return;
        
        case "updatePill":
            request = {
                method: 'put',
                url: URL + "5194/medications/update",
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {
                    Amount: DataHandler.getState("pillAmount"),
                    MedicationId: DataHandler.getState("pillId"),
                },
            }; return;
        
        case "deletePill":
            request = {
                method: 'delete',
                url: URL + "5194/medications/" + DataHandler.getState("pillId") + "?medicineCabinetName=" + DataHandler.getState("medicineCabinetName"),
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {
                },
            }; return;

        case "updatePillDate":
            request = {
                method: 'put',
                url: URL + "5194/update/medication/date",
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {
                    MedicationId: DataHandler.getState("pillId"),
                    MedicationNewDate: DataHandler.getState("pillDate")
                },
            }; return;

        case "removeMember":
            request = {
                method: 'delete',
                url: URL + "5194/cabinet/user/remove-member?targetToRemovePhoneNumber=" + DataHandler.getState("targetPhone") + "&cabinetId=" + DataHandler.getState("cabinetId"),
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {
                },
            }; return;

        case "deleteReminder":
            request = {
                method: 'delete',
                url: URL + "5195/DeleteReminder?Id=" + DataHandler.getState("reminderId"),
                headers: {
                    "Authorization": "Bearer " + user.Token,
                },
                data: {
                },
            }; return;
    

        default:
            console.error("Invalid request type");
    }
}

export default {
    async sendRequest(requestType: string, logging?: boolean, requestedByUI?: boolean) {

        if (!requestedByUI) { // request was made by user directly -- handle spammed requests

            if (alreadyAwaitingResponse) return;
            
            const timeNow = Date.now();
            const deltaTime = timeNow - lastRequestTime;

            if (lastRequestTime && lastRequestType && 
            //(deltaTime < COOLDOWN_PERIOD / COOLDOWN_MULTIPLIER) || // more safe but needs debugging on screen refresh -- need to implement force send request if it was made by the UI and not the user, for this line to work.
            (lastRequestType === requestType && deltaTime < COOLDOWN_PERIOD))
                return; // prevent rapidly repeated requests
            
            alreadyAwaitingResponse = true;    
            lastRequestType = requestType;
            lastRequestTime = timeNow;

        }
        // send request
        try {

            // prevent axios from throwing errors on non-OK status codes recieved
            axios.defaults.validateStatus = function () {
                return true;
            };

            // parse outgoing request
            createRequest(requestType);
            
            if (logging) { 
                console.log(requestType);
                console.log("REQUEST: " + JSON.stringify(request))
            };

            // send request
            response = await axios(request);
            
            // parse response on OK
            if (response.request.status == 200) {
                
                parsedResponse = JSON.parse(response.request._response);
                if (logging) console.log("RESPONSE: " + response.request._response);
                alreadyAwaitingResponse = false;
                return true;

            } else if (response.request.status == 401) {

                if (requestType != "verifySignup" && requestType != "verifyLogin") {
                    DataHandler.expireSession();
                }
                alreadyAwaitingResponse = false;
                return false;

            } else { // other bad error code
                if (logging) {
                    console.log(response);
                }
                else {
                  console.log("Error at: '" + requestType + "' with status code: " + response?.request?.status);
                }
                alreadyAwaitingResponse = false;
                return false;
            }
            
        } catch (error) {
            console.error("Error fetching data:", error);
            response = {} as AxiosResponse<any,any>
            alreadyAwaitingResponse = false;
            return false;
        }
    },
    getRequest() {
        return request;
    },
    getResponse() {
        return response;
    },
    getParsedResponse() {
        return parsedResponse;
    },
    getStatusCode() {
        return response?.request?.status;
    },
    async sleep(ms?: number) {
        return new Promise((resolve) => setTimeout(resolve, ms || DEFAULT_SLEEP_TIME));
    },
};
