import React, { useCallback, useEffect } from 'react';
import { useFocusEffect } from '@react-navigation/native';
import { router } from 'expo-router';

import { View, StyleSheet, Image, ScrollView, Alert, Text } from 'react-native';
import { AppHomeButton } from "@/components/AppHomeButton";
import { MrPillLogo } from '@/components/MrPillLogo';
import { strFC } from "@/components/strFC";
import ParallaxScrollView from "@/components/ParallaxScrollView";
import DataHandler from "@/DataHandler";
import { Pressable } from 'react-native';
import RequestHandler from '@/RequestHandler';
import { PopButton } from '@/components/PopButton';

const backgroundColorLight = "#c9c9ff"
const backgroundColorAlt = "#ffe3e3"
const backgroundColorMain = "#dff5ff"
const borderColor = "#8a8aa7"

function helloMessage() {
  let hours = new Date().getHours();

  if      (hours > 0  && hours <= 10) return "בוקר טוב";
  else if (hours > 10 && hours <= 17) return "צהריים טובים";
  else if (hours > 17 && hours <  22) return "ערב טוב";

  else return "לילה טוב";
}

type Reminder = {
  reminderId: number;
  reminderTime: string;
  recurrenceInterval: string;
  drugHebrewName: string | null;
  imagePath: string;
  medicineCabinetName: string | null;
};

type Notification = {
  id: number,
  targetPhoneNumber: string,
  senderPhoneNumber: string,
  cabinetName: string,
  senderName: string,
  isHandle: boolean,
  isApprove: boolean,
  isSenderSeen: boolean,
  dateStart: string,
  dateEnd: string
}

function getEmoji(type?: number) {
  const emojis = ["😎","😍","🥰","😁","😊","😄","😃","😀","😏"];
  return emojis[type? type % emojis.length : Math.floor(Math.random() * emojis.length)];
}

const HomePage: React.FC = () => {

  const [myReminders, setMyReminders] = React.useState<[Reminder?]>([]);
  const [myNotifications, setNotifications] = React.useState<[Notification?]>([]);
  const [isNotificationsOpen, setIsNotificationsOpen] = React.useState<boolean>(false);
  const [screenUpdated, setScreenUpdated] = React.useState<boolean>();

  const respondToReminder = async (reminderId: number, userResponse: boolean) => {
    DataHandler.setState("reminderId", reminderId.toString());
    DataHandler.setFlag("approveReminder", userResponse);
    if (!await RequestHandler.sendRequest("approveReminder", true)) {
        Alert.alert("שגיאה בעת אישור\\ביטול התראה");
    }
    setScreenUpdated(!screenUpdated);
  }

  const respondToJoinCabinetRequest = async (notification: Notification, userResponse: boolean) => {
    DataHandler.setFlag("userResponse", userResponse);
    DataHandler.set("notification", notification);
    if (await RequestHandler.sendRequest("respondToJoinCabinetRequest", true)) {
        setIsNotificationsOpen(myNotifications.length > 1);    
    } else {
        Alert.alert("שגיאה מספר: " + RequestHandler.getStatusCode() + "\nשגיאה בעת אישור\\דחיית בקשה");
    }
    RequestHandler.sleep(1000);
    setScreenUpdated(!screenUpdated);
  }

  const sendGetRemindersRequest = async () => {
    
    if (await RequestHandler.sendRequest('getMyRemindersToday', false, true)) {
      setMyReminders(JSON.parse(RequestHandler.getResponse().request._response).reverse());
    }
  }

  const sendGetNotificationsRequest = async () => {
              
    if (await RequestHandler.sendRequest('getNotifications', false, true)) {
      setNotifications(JSON.parse(RequestHandler.getResponse().request._response).data);
    }
  }

  const logOut = () => {
    
    Alert.alert(
        "התנתקות משתמש", //title
        "האם ברצונך להתנתק מהמשתמש?", // message
        [
          {
            text: "ביטול",
            onPress: () => {},
            style: "cancel", 
          },
          {
            text: "אישור",
            onPress: () => {
                DataHandler.reset();
                DataHandler.expireSession();
            },
          }
        ],
        { cancelable: true } 
      );
  }

  useEffect(() => {
    if (screenUpdated === undefined) return;

    sendGetRemindersRequest();
    RequestHandler.sleep(1000);
    sendGetNotificationsRequest();

  }, [screenUpdated])

  useFocusEffect(

    useCallback(() => {

      setIsNotificationsOpen(false);
      sendGetRemindersRequest();
      RequestHandler.sleep(1000);
      sendGetNotificationsRequest();
      
      return () => {
        //console.log('Screen was unfocused or navigating away');
      };

    }, [])
  );

  function renderReminder(reminder?: Reminder, id?: number) {
    if (!reminder) return;
    return (
        <View key={id} style={styles.reminderBox}>
            <View style={{width: "20%", marginLeft: 10}}>
                {id == 0 && <Text style={[styles.text]}>שכחתי</Text>}
                <PopButton 
                    ButtonAction={()=>{respondToReminder(reminder.reminderId, false)}}
                    ButtonContent={ 
                        <View style={[styles.plusMinusButton, {elevation: 5, backgroundColor: "#fdfdfd"}]}>
                            <Text style={[styles.plusMinusText, {color: 'red'}]}>❌</Text>
                        </View>
                    }
                />
            </View>
            
            <View style={{width: "20%"}}>
                {id == 0 && <Text style={[styles.text]}>לקחתי</Text>}
                <PopButton 
                    ButtonAction={()=>{respondToReminder(reminder.reminderId, true)}}
                    ButtonContent={ 
                        <View style={[styles.plusMinusButton, {elevation: 5, backgroundColor: "#fdfdfd"}]}>
                            <Text style={[styles.plusMinusText, {color: 'green'}]}>✔</Text>
                        </View>
                    }
                />
            </View>
            
            <View style={{maxWidth: "60%", minWidth: "40%", height: "100%", flex: 1, alignItems: 'center', justifyContent: 'center'}}>
                <Image source={{uri: reminder.imagePath}} style={{borderRadius: 0, height: "50%", width: "100%"}} resizeMode="center"></Image>

                <View>
                    <Text style={{color: "#000", fontSize: 20, fontWeight: 'bold', textAlign: 'center'}}>{reminder.drugHebrewName}</Text>
                    <Text style={{color: "#000", fontSize: 20, textAlign: 'center'}}>{"בשעה " + reminder.reminderTime.slice(11,16)}</Text>
                </View>
            </View>
        </View>
    )
  }

  function renderNotification(notification?: Notification, id?: number) {
    if (!notification) return;
    return (
        
        <View style={styles.reminderBox}>
            <View style={{minWidth: "20%", marginLeft: 10}}>
                <PopButton 
                    ButtonAction={()=>{respondToJoinCabinetRequest(notification, true)}}
                    ButtonContent={
                    <View style={[styles.plusMinusButton, {elevation: 5, backgroundColor: "#FFF"}]}>
                        <Text style={[styles.plusMinusText, {color: 'green'}]}>✔</Text>
                    </View>
                    }
                />
            </View>

            <View style={{minWidth: "20%"}}>
                <PopButton
                    ButtonAction={()=>{respondToJoinCabinetRequest(notification, false)}}
                    ButtonContent={
                    <View style={[styles.plusMinusButton, {elevation: 5, backgroundColor: "#FFF"}]}>
                        <Text style={[styles.plusMinusText, {color: 'red'}]}>❌</Text>
                    </View>    
                    }
                />
            </View>

          <View style={{borderWidth: 2, flex: 1}}>
            <Text style={{color: "#000", fontSize: 20, fontWeight: 'bold', textAlign: 'center'}}>{notification.senderName}{"\n"}0{notification.senderPhoneNumber}</Text>
            <Text style={{color: "#000", fontSize: 16, textAlign: 'center'}}>שלח לך בקשת הצטרפות לארון</Text>
          </View>
        </View>
    )
  }

  const user = DataHandler.getUser()

  // MAIN LAYOUT
  return (
    <View style={{backgroundColor: backgroundColorMain, flex: 1}}>
        
        <View style={[styles.topButtons, {left: "85%"}]}>
            <PopButton  ButtonAction={logOut} ButtonContent= {
                <Text style={{textAlign: 'center', lineHeight: 55, fontSize: 45}}>🚪</Text>
            }/>
        </View>
            
      {myNotifications.length > 0 &&
      <View style={[styles.topButtons, {left: "5%",}]}>
        <PopButton ButtonAction={()=>{setIsNotificationsOpen(!isNotificationsOpen)}} ButtonContent={
        <>
        <Text style={{minWidth: 60, textAlign: 'center', lineHeight: 55, fontSize: 45}}>🔔</Text>
        <Text style={{position: 'absolute', top: "-15%", left: "-25%", lineHeight: 25, fontSize: 15}}>🔴</Text>
        </>}/>
      </View>}

      {MrPillLogo(0.5, true)}

      <Text style={{color: "#000", lineHeight: 30, marginTop: 15, fontSize: 20, textAlign: 'center'}}>{helloMessage()} <Text style={{color: "#000", fontSize: 18, fontWeight: 'bold',}}>{user.FirstName + " " + user.LastName}</Text>!</Text>
      
      {isNotificationsOpen &&
      <View style={{flex: 1,}}>
        <View style={[styles.pagetop, {borderBottomWidth: 10, borderColor: backgroundColorLight, backgroundColor: backgroundColorAlt}]}>
        <Text style={styles.text}>בקשות הצטרפות:</Text>
          <ParallaxScrollView backgroundColor={backgroundColorAlt}>
              {myNotifications.map((notification, index) => renderNotification(notification, index))}
          </ParallaxScrollView>
        </View>
      </View>}

      {!isNotificationsOpen && 
      <View style={{flex: myReminders.length > 0 ? 1 : 0}}>
        <View style={styles.pagetop}> 

          <Text style={styles.text}>{myReminders.length > 0 ? "תזכורות להיום:" : "אין תזכורות להיום  " + getEmoji()}</Text>
 
          {myReminders.length > 0 && 
          <ParallaxScrollView backgroundColor={backgroundColorLight}>
            {myReminders.map((reminder, index) => renderReminder(reminder, index))}
          </ParallaxScrollView>}
        
        </View>
      </View>}


      {// empty flex view for page layout
      (myNotifications.length < 1 && isNotificationsOpen) || (myReminders.length < 1 && !isNotificationsOpen)
      && <View style={{flex: 1}}/>
      }
              
      <View style={styles.pagebottom}>

        <View style={styles.row}>
          <AppHomeButton key={1} Type={1} ButtonContent={strFC("הארונות שלי")} ButtonAction={()=>{router.push('/(cabinet)/mycabinets')}}/>
          <AppHomeButton key={2} Type={2} ButtonContent={strFC("התרופות שלי")} ButtonAction={()=>{router.push('/(pills)/mypills')}}/>
        </View>

        <View style={styles.row}>
          <AppHomeButton key={3} Type={3} ButtonContent={strFC("תזכורות")} ButtonAction={()=>{router.push('/(reminders)/reminders')}}/>
          <AppHomeButton key={4} Type={4} ButtonContent={strFC("הוסף תרופה חדשה")} ButtonAction={()=>{router.push('/(pills)/addpill')}}/>
        </View>

      </View>

    </View>

  );
};

const styles = StyleSheet.create({
  pagetop: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: backgroundColorLight,
    borderRadius: 20,
    //borderWidth: 2,
    borderColor: borderColor,
    minHeight: 75,
    marginHorizontal: 15,
    padding: 5,
    elevation: 5,
  },
  topButtons: {
    zIndex: 5,
    minHeight: 50,
    minWidth: 50,
    backgroundColor: "#dddd",
    position: 'absolute',
    marginTop: "14%",
    borderRadius: 999,
    elevation: 3
  },
  pagebottom: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    borderRadius: 20,
    minHeight: 150,
    marginHorizontal: 15,
    marginVertical: 20,
    padding: 5,
  },
  row: {
    flex: 1,
    minHeight: 170,
    justifyContent: 'center',
    alignItems: 'center',
    flexDirection: 'row',
  },
  text: {
    textAlign: 'center',
    lineHeight: 35,
    fontSize: 25,
    fontWeight: 'bold',
    color: '#000',
  },
  reminderBox: {
    padding: 3,
    backgroundColor: "#dadada",
    flexDirection: "row",
    borderColor: borderColor,
    borderRadius: 12,
    //borderWidth: 2,
    gap: 15,
    flex: 1,
    justifyContent: "space-between",
    alignItems: "center",
    minHeight: 120,
    minWidth: "100%",
    elevation: 3,
  },
  plusMinusButton: {
    width: 75,
    height: 75,
    borderRadius: 999,
    justifyContent: 'center',
    alignItems: 'center',
    margin: 5,
    elevation: 3,
  },
  plusMinusText: {
    fontSize: 35,
    lineHeight: 45,
    fontWeight: 'bold',
    position: 'absolute',
  }
});

export default HomePage;
