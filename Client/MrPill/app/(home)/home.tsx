import React from 'react';
import { router } from 'expo-router';
import { ThemedText } from '@/components/ThemedText';
import { View, StyleSheet } from 'react-native';
import { AppHomeButton } from "@/components/AppHomeButton";
import { MrPillLogo } from '@/components/MrPillLogo';
import { strFC } from "@/components/strFC";
import ParallaxScrollView from "@/components/ParallaxScrollView";
import DataHandler from "@/DataHandler";

const bgc = "#b8eaff"

const HomePage: React.FC = () => {

  const user = DataHandler.getUser()

  return (
    <ParallaxScrollView headerBackgroundColor={{ light: bgc, dark: bgc }} headerImage={MrPillLogo()} backgroundColor={bgc}>
      <View style={{backgroundColor: bgc}}>
      <View style={styles.pagetop}> 

        <ThemedText style={{fontSize: 18, marginBottom: 10}}>שלום <ThemedText style={{fontSize: 18, fontWeight: 'bold',}}>{user.FirstName + " " + user.LastName}</ThemedText>. התרופות הקרובות:</ThemedText>

        {/* need to implement dynamic upcoming medication */}
        <View style={styles.reminderBox}>
          <ThemedText>Calcium carbonate</ThemedText>
          <ThemedText>בשעה 14:00 לאחר האוכל </ThemedText>
        </View>

      </View>

      <View style={{marginVertical: 40}}></View>

      <View style={styles.container}>
        <View style={styles.row}>
          <AppHomeButton ButtonContent={strFC("הוסף ארון תרופות")} ButtonAction={()=>{router.navigate('/(home)/sharedpills')}}/>
          <AppHomeButton ButtonContent={strFC("התרופות שלי")} ButtonAction={()=>{router.navigate('/(home)/mypills')}}/>
        </View>

        <View style={styles.row}>
          <AppHomeButton ButtonContent={strFC("תזכורות")} ButtonAction={()=>{router.navigate('/(home)/reminders')}}/>
          <AppHomeButton ButtonContent={strFC("הוסף תרופה חדשה")} ButtonAction={()=>{router.navigate('/(home)/addpill')}}/>
        </View>
      </View>
      </View>
    </ParallaxScrollView>

  );
};

const styles = StyleSheet.create({
  pagetop: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#dcffff',
    borderRadius: 20,
    minHeight: 50,
    padding: 5,
  },
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  row: {
    flex: 1,
    minHeight: 170,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: bgc,
    flexDirection: 'row',
  },
  text: {
    fontSize: 20,
    color: '#000',
  },
  reminderBox: {
    backgroundColor: 'pink',
    borderRadius: 12,
    flex: 1,
    justifyContent: 'center',
    alignContent: 'center',
    paddingHorizontal: 15,
    paddingVertical: 5,
  },
});

export default HomePage;
