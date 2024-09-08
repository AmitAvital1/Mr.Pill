
import React, { useCallback, useEffect, useState } from 'react';
import { router } from 'expo-router';

import { View, StyleSheet, Alert, Text } from 'react-native';
import { AppHomeButton } from "@/components/AppHomeButton";
import { MrPillLogo } from '@/components/MrPillLogo';
import { strFC } from "@/components/strFC";
import ParallaxScrollView from "@/components/ParallaxScrollView";
import DataHandler from "@/DataHandler";
import { Pressable, Image } from 'react-native';

import RequestHandler from '@/RequestHandler';
import { useFocusEffect } from '@react-navigation/native';

function getFamilyEmoji(type?: number) {
    const familyEmojis = ["👪","👨‍👩‍👦","👨‍👩‍👧","👨‍👩‍👧‍👦","👨‍👩‍👦‍👦","👨‍👩‍👧‍👧","👨‍👨‍👦","👨‍👨‍👧","👩‍👩‍👧‍👧","👩‍👩‍👦‍👦","👩‍👩‍👧‍👦","👩‍👩‍👧","👩‍👩‍👦","👨‍👨‍👧‍👧","👨‍👨‍👦‍👦","👨‍👨‍👧‍👦","👨‍👧‍👦","👨‍👧","👨‍👦","👩‍👧‍👧","👩‍👦‍👦","👩‍👧‍👦","👩‍👧","👩‍👦","👨‍👦‍👦","👨‍👧‍👧"];
    return familyEmojis[type? type % 26 : Math.floor(Math.random() * 26)];
}

type Reminder = {
  reminderId: number;
  reminderTime: string;
  recurrenceInterval: string;
  drugHebrewName: string | null;
  imagePath: string;
  medicineCabinetName: string | null;
}

const backgroundColorLight = "#71bfe9"
const backgroundColorMain = "#e6c8c8"
const borderColor = "#005a27"

const MyReminders: React.FC = () => {

  const [myReminders, setMyReminders] = React.useState<[Reminder?]>([]);
  const [isDeleteEnabled, setIsDeleteEnabled] = React.useState<boolean>(false);
  const [screenUpdated, setScreenUpdated] = React.useState<boolean>(false);

  const handleDeleteReminderButtonPress = async (reminder: Reminder) => {

    DataHandler.setState("reminderId", reminder.reminderId.toString());
    if (await RequestHandler.sendRequest("deleteReminder")) {
      Alert.alert("תזכורת נמחקה בהצלחה!");
      setIsDeleteEnabled(false);
      setScreenUpdated(!screenUpdated);
    } else {
      Alert.alert("שגיאה במחיקת התזכורת");
    }

  }

  function renderReminder(reminder?: Reminder, id?: number) {
    if (!reminder) return;
    return (
      <View style={styles.reminderBox}>

        <View style={{alignItems: 'center', flexDirection: 'row'}}>
          {isDeleteEnabled &&
          <Pressable onPress={()=>handleDeleteReminderButtonPress(reminder)} style={{borderWidth: 2, borderColor: "grey", marginRight: 20, justifyContent: 'center', alignContent: 'center', minHeight: 50, minWidth: 50, backgroundColor: "#da5454", borderRadius: 999}}>
            <Text style={{fontSize: 22, fontWeight: 'bold', textAlign: 'center'}}>הסר</Text>
          </Pressable>}

          <View style={[styles.plusMinusButton, {elevation: 5, backgroundColor: "#90e665"}]}>
            <Image source={{uri: reminder.imagePath}} style={{height: 50, width: 50}} resizeMode='center'></Image>
          </View>
            
          <View style={{flexGrow: 1}}>
            <Text style={{fontSize: 25, fontWeight: 'bold', marginRight: 35, textAlign: 'center'}}>{reminder.drugHebrewName}</Text>
            <Text style={{fontSize: 20, marginRight: 35, textAlign: 'center'}}>{"בשעה " + reminder.reminderTime.slice(11,16) + "\nבתאריך " + reminder.reminderTime.slice(0,10)}</Text>
          </View>

        </View>

      </View>
    )
  }

  const sendGetRemindersRequest = async () => {
    if (await RequestHandler.sendRequest('getMyReminders')) {
      setMyReminders(JSON.parse(RequestHandler.getResponse().request._response));
    }
  };

  useEffect(()=>{
    sendGetRemindersRequest();
  },[screenUpdated])

  useFocusEffect(
    useCallback(() => {
      sendGetRemindersRequest();

      return () => {
        //console.log('Screen was unfocused or navigating away');
      };
    }, [])
  );

  // MAIN PAGE LAYOUT
  return (    
    <View style={{backgroundColor: backgroundColorMain, flex: 1}}>
        <View style={{flex: 1}}>
        {MrPillLogo(0.5)}
            <View style={styles.pagetop}> 
                <Text style={{lineHeight: 34, textAlign: 'center', fontSize: 24, textDecorationLine: 'underline', fontWeight: 'bold', marginTop: 8}}>
                    התזכורות שלי:
                </Text>
                {myReminders.length < 1 &&
                <Text style={{color: "#d47211", lineHeight: 34, textAlign: 'center', fontSize: 20, fontWeight: 'bold', marginTop: 8}}>
                אין תזכורות פעילות.
                </Text>}
                <ParallaxScrollView backgroundColor={backgroundColorLight}>
                  {myReminders.map((reminder, index) => renderReminder(reminder, index))}
                </ParallaxScrollView>
            </View>
        </View>
        
        <View style={styles.pagebottom}>
          <View style={styles.row}>
            <AppHomeButton BackgroundColor={"#da5454"} BorderColor={"grey"} ButtonContent={strFC("מחיקת תזכורת")} ButtonAction={()=>{setIsDeleteEnabled(!isDeleteEnabled)}}/>
            <AppHomeButton BackgroundColor={"lightgreen"} BorderColor={"grey"} ButtonContent={strFC("הוסף תזכורת חדשה")} ButtonAction={()=>{router.navigate('/(reminders)/addreminder')}}/>
          </View>
        </View>

    </View>
  );
};

const styles = StyleSheet.create({
  pagetop: {
    flex: 1,
    marginTop: 20,
    justifyContent: 'flex-start',
    alignItems: 'center',
    backgroundColor: backgroundColorLight,
    borderRadius: 20,
    borderWidth: 2,
    borderColor: borderColor,
    minHeight: 100,
    marginHorizontal: 15,
    paddingBottom: 5,
    paddingHorizontal: 5,
    elevation: 8,
  },
  pagebottom: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    borderRadius: 20,
    marginHorizontal: 15,
    marginVertical: 20,
    padding: 5,
    maxHeight: 180
  },
  row: {
    flex: 1,
    minHeight: 5,
    justifyContent: 'center',
    alignItems: 'center',
    flexDirection: 'row',
  },
  text: {
    fontSize: 20,
    color: '#000',
  },
  reminderBox: {
    backgroundColor: 'pink',
    borderWidth: 2,
    borderColor: borderColor,
    borderRadius: 12,
    flex: 1,
    justifyContent: 'center',
    alignContent: 'center',
    paddingHorizontal: 15,
    paddingVertical: 5,
    minWidth: "90%",
    minHeight: 100
  },
  plusMinusButton: {
    minWidth: 50,
    minHeight: 50,
    borderRadius: 25,
    borderWidth: 2,
    borderColor: backgroundColorLight,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 12,
    elevation: 5,
  },
  plusMinusText: {
    fontSize: 30,
    fontWeight: 'bold',
    position: 'absolute',
  }
});

export default MyReminders;
