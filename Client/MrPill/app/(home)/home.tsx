import React, { useCallback } from 'react';
import { useFocusEffect } from '@react-navigation/native';
import { router } from 'expo-router';
import { ThemedText } from '@/components/ThemedText';
import { View, StyleSheet, Image } from 'react-native';
import { AppHomeButton } from "@/components/AppHomeButton";
import { MrPillLogo } from '@/components/MrPillLogo';
import { strFC } from "@/components/strFC";
import ParallaxScrollView from "@/components/ParallaxScrollView";
import DataHandler from "@/DataHandler";
import { Pressable } from 'react-native';
import RequestHandler from '@/RequestHandler';

const backgroundColorLight = "#c9c9ff"
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

const HomePage: React.FC = () => {

  const [myReminders, setMyReminders] = React.useState<[Reminder?]>([]);

  useFocusEffect(
    useCallback(() => {
    
      //
      const sendGetRemindersRequest = async () => {
              
        if (await RequestHandler.sendRequest('getMyReminders')) {
          setMyReminders(JSON.parse(RequestHandler.getResponse().request._response));
        }
      }
      sendGetRemindersRequest();
      //

      return () => {
        //console.log('Screen was unfocused or navigating away');
      };
    }, [])
  );

  function renderReminder(reminder?: Reminder, id?: number) {
    if (!reminder || !id) return;
    if (reminder.reminderTime.slice())
    return (
      <Pressable key={id} onPress={()=>{console.log('y')}}>
        
        <View style={styles.reminderBox}>
          <View style={{alignItems: 'center', flexDirection: 'row'}}>
      
            <View style={[styles.plusMinusButton, {elevation: 5, backgroundColor: backgroundColorLight}]}>
              <Image source={{uri: reminder.imagePath}} style={{borderRadius: 25, height: 100, width: 100}} resizeMode='center'></Image>
            </View>
              
            <View style={{flexGrow: 1}}>
              <ThemedText style={{fontSize: 20, fontWeight: 'bold', marginRight: 35, textAlign: 'center'}}>{reminder.drugHebrewName}</ThemedText>
              {//<ThemedText style={{marginRight: 35, textAlign: 'center'}}>{reminder.message}</ThemedText>}
              }
              <ThemedText style={{fontSize: 20, marginRight: 35, textAlign: 'center'}}>{"בשעה " + reminder.reminderTime.slice(11,16) + "\n בתאריך " + reminder.reminderTime.slice(0,10)}</ThemedText>
            </View>
    
          </View>
        </View>

      </Pressable>
    )
  }

  const user = DataHandler.getUser()

  return (
    <View style={{backgroundColor: backgroundColorMain, flex: 1}}>

      <View style={{flex: 1, minHeight: 40}}>
        {MrPillLogo(1)}
      </View>
      <View style={{flex: 1, minHeight: 50,}}>
        <View style={styles.pagetop}> 

          <ThemedText style={{fontSize: 18, textAlign: 'center'}}>{helloMessage()} <ThemedText style={{fontSize: 18, fontWeight: 'bold',}}>{user.FirstName + " " + user.LastName + ".\n"}</ThemedText>תזכורות להיום:</ThemedText>
          <ParallaxScrollView backgroundColor={backgroundColorLight}>
            {myReminders.map((reminder, index) => renderReminder(reminder, index))}
          </ParallaxScrollView>
        
        </View>
      </View>
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
    justifyContent: 'flex-start',
    alignItems: 'center',
    backgroundColor: backgroundColorLight,
    borderRadius: 20,
    //borderWidth: 2,
    borderColor: borderColor,
    minHeight: 100,
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
    backgroundColor: backgroundColorMain,
    flexDirection: 'row',
  },
  text: {
    fontSize: 20,
    color: '#000',
  },
  reminderBox: {
    backgroundColor: 'pink',
    //borderWidth: 2,
    borderColor: borderColor,
    borderRadius: 12,
    flex: 1,
    justifyContent: 'center',
    alignContent: 'center',
    paddingHorizontal: 15,
    paddingVertical: 5,
    minWidth: 300,
  },
  plusMinusButton: {
    minWidth: 50,
    minHeight: 50,
    borderRadius: 25,
    //borderWidth: 2,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 12,
  },
  plusMinusText: {
    fontSize: 35,
    fontWeight: 'bold',
    position: 'absolute',
  }
});

export default HomePage;
