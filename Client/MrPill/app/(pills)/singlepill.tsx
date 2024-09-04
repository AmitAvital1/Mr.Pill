import React from 'react';
import { Redirect, router } from 'expo-router';
import { ThemedText } from '@/components/ThemedText';
import { View, StyleSheet, Image, Pressable } from 'react-native';
import { AppHomeButton } from "@/components/AppHomeButton";
import { MrPillLogo } from '@/components/MrPillLogo';
import { strFC } from "@/components/strFC";
import DataHandler from '@/DataHandler';
import { PopButton } from '@/components/PopButton';

import RequestHandler from '@/RequestHandler';
import { WebView } from 'react-native-webview';

const backgroundColorLight = "#cbc4ff"
const backgroundColorMain = "#c8e3e6"
const borderColor = "#02005a55"

type Pill = {
    id: number;
    englishName: string | null;
    hebrewName: string | null;
    englishDescription: string | null;
    hebrewDescription: string | null;
    validity: string | null;
    userId: number;
    medicationRepoId: number;
    imagePath: string;
    isPrivate: boolean;
    numberOfPills: number;
    shelfLife: number;
    medicineCabinetName: string | null;
    brochurePath: string;
}

const SinglePillPage: React.FC = () => {

  const pill: Pill = DataHandler.get('pill');
  const [screenUpdated, setScreenUpdated] = React.useState<boolean>();

  const handlePlusButtonPress = async () => {
    DataHandler.setState("pillId", pill.id.toString());
    DataHandler.setState("pillAmount", "1");
    if (await RequestHandler.sendRequest("updatePill"))
      pill.numberOfPills++;
    setScreenUpdated(!screenUpdated);
  }

  const handleMinusButtonPress = async () => {
    DataHandler.setState("pillId", pill.id.toString());
    DataHandler.setState("pillAmount", "-1");
    if (await RequestHandler.sendRequest("updatePill"))
      pill.numberOfPills--;
    setScreenUpdated(!screenUpdated);
  }

  const handlePillImagePress = () => {
    DataHandler.setState("pdfURL", pill.brochurePath);
    router.push("/(pills)pillbrochure");
  }

  // MAIN PAGE LAYOUT
  return (    
    <View style={{backgroundColor: backgroundColorMain, flex: 1}}>
        <View style={{flex: 1}}>
        {MrPillLogo(0.5)}
        
            <View style={styles.pagetop}>
                
                <View style={styles.imageContainer}>
                  <ThemedText style={styles.text}>{pill.hebrewName}</ThemedText>
                    <Image source={{uri: pill.imagePath}} style={styles.image} resizeMode="center"/>
                  <ThemedText style={styles.text}>{pill.hebrewDescription}</ThemedText>
                </View>

                <ThemedText style={styles.text}>מתוך ארון התרופות: {pill.medicineCabinetName}</ThemedText>
                <View style={{flex: 1}}/>
                <ThemedText style={styles.text}>מספר התרופות שנותרו:  <ThemedText style={[styles.text, {lineHeight: 40, fontSize: 40, color: pill.numberOfPills < 6 ? "red" : (pill.numberOfPills < 11 ? "yellow" : "green")}]}>{pill.numberOfPills}</ThemedText></ThemedText>
            </View>
        </View>
        
        <View style={styles.pagebottom}>
            <View style={styles.row}>
                <AppHomeButton BackgroundColor={backgroundColorLight} BorderColor={borderColor} ButtonContent={strFC("➕", 50)} ButtonAction={()=>{handlePlusButtonPress()}}/>
            </View>
            <View style={styles.row}>
                <AppHomeButton BackgroundColor={backgroundColorLight} BorderColor={borderColor} ButtonContent={strFC("➖", 50)} ButtonAction={()=>{handleMinusButtonPress()}}/>
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
    borderColor: borderColor,
    minHeight: 100,
    marginHorizontal: 15,
    paddingBottom: 5,
    paddingHorizontal: 5,
    gap: 5,
    elevation: 3,
  },
  pagebottom: {
    flex: 1,
    flexDirection: 'row',
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
    lineHeight: 36,
    fontSize: 26,
    fontWeight: 'bold',
    color: '#000',
    textAlign: 'center',
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
  image: {
    flex: 1,
    alignSelf: "center",
    height: "100%",
    width: "100%",
  }, 
  imageContainer: {
    margin: 8,
    minHeight: "20%",
    maxHeight: "50%",
    flex: 1,
    width: "100%",
    borderRadius: 20,

    borderColor: "#747474",
    backgroundColor: "#bdddf3",
    justifyContent: 'center',
    alignContent: 'center',
    overflow: 'hidden',
    
    elevation: 4  ,
    gap: 3,
  },
});

export default SinglePillPage;