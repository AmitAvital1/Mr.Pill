import React, { useCallback, useEffect } from 'react';
import { useFocusEffect } from '@react-navigation/native';
import { router } from 'expo-router';
import { ThemedText } from '@/components/ThemedText';
import { View, StyleSheet, Image, ScrollView } from 'react-native';
import { AppHomeButton } from "@/components/AppHomeButton";
import { MrPillLogo } from '@/components/MrPillLogo';
import { strFC } from "@/components/strFC";
import ParallaxScrollView from "@/components/ParallaxScrollView";
import DataHandler from "@/DataHandler";
import { Pressable } from 'react-native';
import RequestHandler from '@/RequestHandler';

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

  const respondToJoinCabinetRequest = async (notification: Notification, userResponse: boolean) => {
    DataHandler.setFlag("userResponse", userResponse);
    DataHandler.set("notification", notification);

    await RequestHandler.sendRequest("respondToJoinCabinetRequest");
    setScreenUpdated(!screenUpdated);
  }

  const sendGetRemindersRequest = async () => {
              
    if (await RequestHandler.sendRequest('getMyRemindersToday')) {
      setMyReminders(JSON.parse(RequestHandler.getResponse().request._response));
    }
  }

  const sendGetNotificationsRequest = async () => {
              
    if (await RequestHandler.sendRequest('getNotifications')) {
      setNotifications(JSON.parse(RequestHandler.getResponse().request._response).data);
      console.log(RequestHandler.getResponse().request._response)
    }
  }

  const logOut = () => {
    DataHandler.reset();
    DataHandler.expireSession();
  }

  useEffect(() => {
    if (screenUpdated === undefined) return;

    const updateNotifications = async ()=> {
      await sendGetNotificationsRequest();
      setIsNotificationsOpen(myNotifications.length > 0);
    }

    updateNotifications();
  }, [screenUpdated])

  useFocusEffect(

    useCallback(() => {

      setIsNotificationsOpen(false);
      sendGetRemindersRequest();
      sendGetNotificationsRequest();
      
      return () => {
        //console.log('Screen was unfocused or navigating away');
      };

    }, [])
  );

  function renderReminder(reminder?: Reminder, id?: number) {
    if (!reminder) return;
    return (
      <Pressable key={id} onPress={()=>{console.log('y')}}>
        
        <View style={styles.reminderBox}>
          <View style={{alignItems: 'center', flexDirection: 'row'}}>

            <Image source={{uri: reminder.imagePath}} style={{borderRadius: 25, height: 100, width: 100, marginRight: 30}} resizeMode='center'></Image>

            <View style={{flexGrow: 1}}>
              <ThemedText style={{fontSize: 20, fontWeight: 'bold', marginRight: 35, textAlign: 'center'}}>{reminder.drugHebrewName}</ThemedText>
              <ThemedText style={{fontSize: 20, marginRight: 35, textAlign: 'center'}}>{"בשעה " + reminder.reminderTime.slice(11,16)}</ThemedText>
            </View>
    
          </View>
        </View>

      </Pressable>
    )
  }

  function renderNotification(notification?: Notification, id?: number) {
    if (!notification) return;
    return (
      <Pressable key={id} onPress={()=>{console.log('y')}}>
        
        <View style={styles.reminderBox}>
          <View style={{alignItems: 'center', flexDirection: 'row'}}>

          <Pressable onPress={()=>{respondToJoinCabinetRequest(notification, true)}}>
            <View style={[styles.plusMinusButton, {elevation: 5, backgroundColor: "#FFF"}]}>
              <ThemedText style={[styles.plusMinusText, {paddingTop: 19.5, color: 'green'}]}>✔</ThemedText>
            </View>
          </Pressable>

          <Pressable onPress={()=>{respondToJoinCabinetRequest(notification, false)}}>
            <View style={[styles.plusMinusButton, {elevation: 5, backgroundColor: "#FFF"}]}>
              <ThemedText style={[styles.plusMinusText, {paddingTop: 17.5}]}>❌</ThemedText>
            </View>
          </Pressable>

          <View style={{width: "50%", flexGrow: 1}}>
            <ThemedText style={{fontSize: 20, fontWeight: 'bold', marginRight: 35, textAlign: 'center'}}>{notification.senderName} - 0{notification.senderPhoneNumber}</ThemedText>
            <ThemedText style={{fontSize: 16, marginRight: 35, textAlign: 'center'}}>שלח לך בקשת הצטרפות לארון</ThemedText>
          </View>
    
          </View>
        </View>

      </Pressable>
    )
  }

  const user = DataHandler.getUser()

  return (
    <View style={{backgroundColor: backgroundColorMain, flex: 1}}>
     
      <Pressable onPress={logOut}>
      <View style={{backgroundColor: "#ddd9", position: 'absolute', marginLeft: "3.5%", marginTop: "7%", borderRadius: 999}}>
        <ThemedText style={{lineHeight: 55, fontSize: 45}}>🚪</ThemedText>
      </View>
      </Pressable>

      {myNotifications.length > 0 &&
      <Pressable onPress={()=>{setIsNotificationsOpen(!isNotificationsOpen)}}>
      <View style={{backgroundColor: "#ddd", position: 'absolute', marginLeft: "5%", marginTop: "30%", borderRadius: 999}}>
        <ThemedText style={{lineHeight: 55, fontSize: 35}}>🔔</ThemedText>
      </View>
      <View style={{position: 'absolute', marginLeft: "5%", marginTop: "30%", borderRadius: 1000}}>
        <ThemedText>🔴</ThemedText>
      </View>
      </Pressable>}
      {MrPillLogo(0.75)}
      <ThemedText style={{marginBottom: 5, fontSize: 18, textAlign: 'center'}}>{helloMessage()} <ThemedText style={{fontSize: 18, fontWeight: 'bold',}}>{user.FirstName + " " + user.LastName}</ThemedText>!</ThemedText>
      {isNotificationsOpen &&
      <View style={{flex: 1,}}>
        <View style={[styles.pagetop, {borderBottomWidth: 10, borderColor: backgroundColorLight, backgroundColor: backgroundColorAlt}]}>
        <ThemedText style={{fontSize: 18, textAlign: 'center'}}>בקשות הצטרפות:</ThemedText>
          <ParallaxScrollView backgroundColor={backgroundColorAlt}>
              {myNotifications.map((notification, index) => renderNotification(notification, index))
              }
          </ParallaxScrollView>
        </View>
      </View>}
      
      {!isNotificationsOpen && 
      <View style={{flex: myReminders.length > 0 ? 1 : 0}}>
        <View style={styles.pagetop}> 

          <ThemedText style={styles.text}>{myReminders.length > 0 ? "תזכורות להיום:" : "אין תזכורות להיום" + getEmoji()}</ThemedText>
 
          {myReminders.length > 0 && 
          <ParallaxScrollView backgroundColor={backgroundColorLight}>
            {myReminders.map((reminder, index) => renderReminder(reminder, index))}
          </ParallaxScrollView>}
        
        </View>
      </View>}
              
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
    lineHeight: 35,
    fontSize: 25,
    fontWeight: 'bold',
    color: '#000',
  },
  reminderBox: {
    backgroundColor: "#dadadadf",
    //borderWidth: 2,
    borderColor: borderColor,
    borderRadius: 12,
    flex: 1,
    justifyContent: 'center',
    alignContent: 'center',
    paddingHorizontal: 15,
    paddingVertical: 5,
    minWidth: 300,
    elevation: 3,
  },
  plusMinusButton: {
    minWidth: 75,
    minHeight: 75,
    borderRadius: 25,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 12,
    elevation: 3,
  },
  plusMinusText: {
    fontSize: 35,
    fontWeight: 'bold',
    position: 'absolute',
  }
});

export default HomePage;
