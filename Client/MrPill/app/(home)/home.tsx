import React from 'react';
import { router } from 'expo-router';
import { ThemedText } from '@/components/ThemedText';
import { View, StyleSheet } from 'react-native';
import { AppHomeButton } from "@/components/AppHomeButton";
import { MrPillLogo } from '@/components/MrPillLogo';
import { strFC } from "@/components/strFC";
import ParallaxScrollView from "@/components/ParallaxScrollView";
import DataHandler from "@/DataHandler";
import { Pressable } from 'react-native';

const backgroundColorLight = "#c9c9ff"
const backgroundColorMain = "#dff5ff"
const borderColor = "#8a8aa7"

function helloMessage() {
  let hours = new Date().getHours();
  console.log(hours)
  if      (hours > 0  && hours <= 10) return "בוקר טוב";
  else if (hours > 10 && hours <= 17) return "צהריים טובים";
  else if (hours > 17 && hours <  22) return "ערב טוב";

  else return "לילה טוב";
}

type Reminder = {
  userId: number;
  reminderTime: string;
  message?: string;
  isRecurring: boolean;
  recurrenceInterval?: string;
};

const HomePage: React.FC = () => {

  function renderReminder(name: string, comment: string){//(reminder?: Reminder) {
    return (
      <View style={styles.reminderBox}>
        <View style={{flexDirection: 'row'}}>
  
        <Pressable onPress={()=>{}}>
          <View style={[styles.plusMinusButton, {backgroundColor: "#90e665"}]}>
            <ThemedText style={[styles.plusMinusText, {paddingTop: 21.5}]}>✔</ThemedText>
          </View>
        </Pressable>
  
        <Pressable onPress={()=>{}}>
          <View style={[styles.plusMinusButton, {backgroundColor: "#cc4e4e"}]}>
            <ThemedText style={[styles.plusMinusText, {paddingTop: 18}]}>✖</ThemedText>
          </View>
        </Pressable>
  
        <View style={{flexGrow: 1}}>
          <ThemedText style={{fontWeight:'bold', textAlign: 'center'}}>{name}</ThemedText>
          <ThemedText style={{textAlign: 'center'}}>{comment}</ThemedText>
        </View>
  
        </View>
      </View>
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
            {renderReminder("Calcium Carbonate 5mg", "14:00 אחרי האוכל")}
            {renderReminder("Donepezil 20mg", "21:00 לפני האוכל")}
            {renderReminder("Donepezil 20mg", "21:00")}
            {renderReminder("Donepezil 20mg", "21:00")}
          </ParallaxScrollView>
        
        </View>
      </View>
      <View style={styles.pagebottom}>

        <View style={styles.row}>
          <AppHomeButton Type={1} ButtonContent={strFC("הארונות שלי")} ButtonAction={()=>{DataHandler.setState('move', 0); router.navigate('/(cabinet)/mycabinets')}}/>
          <AppHomeButton Type={2} ButtonContent={strFC("התרופות שלי")} ButtonAction={()=>{router.navigate('/(pills)/mypills')}}/>
        </View>

        <View style={styles.row}>
          <AppHomeButton Type={3} ButtonContent={strFC("תזכורות")} ButtonAction={()=>{router.navigate('/(reminders)/reminders')}}/>
          <AppHomeButton Type={4} ButtonContent={strFC("הוסף תרופה חדשה")} ButtonAction={()=>{DataHandler.setState('move', 1); router.navigate('/(pills)/addpill')}}/>
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
    borderWidth: 2,
    borderColor: borderColor,
    minHeight: 100,
    marginHorizontal: 15,
    padding: 5,
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
    borderWidth: 2,
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
    borderWidth: 2,
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
