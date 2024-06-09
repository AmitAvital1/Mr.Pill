import React from 'react';
import { router } from 'expo-router';
import { ThemedText } from '@/components/ThemedText';
import { View, StyleSheet } from 'react-native';
import { AppHomeButton } from "@/components/AppHomeButton";
import { MrPillLogo } from '@/components/MrPillLogo';
import { strFC } from "@/components/strFC";
import ParallaxScrollView from "@/components/ParallaxScrollView";

const HomePage: React.FC = () => {

  const userName = `שרה`;

  return (
    <ParallaxScrollView headerBackgroundColor={{ light: "white", dark: "white" }} headerImage={MrPillLogo()}>
    
      <View style={styles.pagetop}> 
        <ThemedText>שלום {userName}.</ThemedText>
      </View>


      <View style={styles.container}>
        <View style={styles.row}>
          <AppHomeButton ButtonContent={strFC("מלאי משפחתי")} ButtonAction={()=>{router.navigate('(home)/sharedpills')}}/>
          <AppHomeButton ButtonContent={strFC("התרופות שלי")} ButtonAction={()=>{router.navigate('(home)/mypills')}}/>
        </View>

        <View style={styles.row}>
          <AppHomeButton ButtonContent={strFC("תזכורות")} ButtonAction={()=>{router.navigate('(home)/reminders')}}/>
          <AppHomeButton ButtonContent={strFC("הוסף תרופה חדשה")} ButtonAction={()=>{router.navigate('(home)/addpill')}}/>
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
    backgroundColor: '#aaf8f8',
    borderRadius: 20,
    minHeight: 50,
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
    backgroundColor: 'white',
    flexDirection: 'row',
  },
  text: {
    fontSize: 20,
    color: '#333',
  },
});

export default HomePage;
