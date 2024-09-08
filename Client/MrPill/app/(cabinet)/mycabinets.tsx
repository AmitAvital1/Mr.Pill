import React, { useCallback } from 'react';
import { router } from 'expo-router';
import { ThemedText } from '@/components/ThemedText';
import { View, StyleSheet, Text } from 'react-native';
import { AppHomeButton } from "@/components/AppHomeButton";
import { MrPillLogo } from '@/components/MrPillLogo';
import { strFC } from "@/components/strFC";
import ParallaxScrollView from "@/components/ParallaxScrollView";
import { Pressable } from 'react-native';
import RequestHandler from '@/RequestHandler';
import { useFocusEffect } from '@react-navigation/native';
import DataHandler from '@/DataHandler';

type Cabinet = {
  id: number,
  medicineCabinetName: string,
  creatorId: number,
  isCreator: boolean
};

function getFamilyEmoji(type?: number) {
    const familyEmojis = ["👪","👨‍👩‍👦","👨‍👩‍👧","👨‍👩‍👧‍👦","👨‍👩‍👦‍👦","👨‍👩‍👧‍👧","👨‍👨‍👦","👨‍👨‍👧","👩‍👩‍👧‍👧","👩‍👩‍👦‍👦","👩‍👩‍👧‍👦","👩‍👩‍👧","👩‍👩‍👦","👨‍👨‍👧‍👧","👨‍👨‍👦‍👦","👨‍👨‍👧‍👦","👨‍👧‍👦","👨‍👧","👨‍👦","👩‍👧‍👧","👩‍👦‍👦","👩‍👧‍👦","👩‍👧","👩‍👦","👨‍👦‍👦","👨‍👧‍👧"];
    return familyEmojis[type? type % 26 : Math.floor(Math.random() * 26)];
}

const backgroundColorLight = "#e9a771"
const backgroundColorMain = "#c8e6d5"
const borderColor = "#005a27"

const MyCabinets: React.FC = () => {

    const [myCabinets, setMyCabinets] = React.useState<[Cabinet?]>([]);

    const sendGetCabinetsRequest = async () => {
        if (await RequestHandler.sendRequest('getMyCabinets', false, true)) {
          setMyCabinets(JSON.parse(RequestHandler.getResponse().request._response));
        }
      };

    useFocusEffect(
      useCallback(() => {
    
        sendGetCabinetsRequest();
  
        return () => {
          //console.log('Screen was unfocused or navigating away');
        };
      }, [])
    );

    function renderCabinet(cabinet: Cabinet | undefined, id: number) {
      if (!cabinet) return;
      return (
        <Pressable key={id} onPress={()=>{console.log('y')}}>
          <View style={styles.reminderBox}>
            <View style={{alignItems: 'center', flexDirection: 'row'}}>
              
              {/*  */}
              <Pressable onPress={()=>{DataHandler.setState("showFromCabinet", cabinet.medicineCabinetName); router.navigate("/(pills)/mypills");}}>
                <View style={[styles.plusMinusButton, {backgroundColor: "#90e665"}]}>
                  <Text style={[styles.plusMinusText, {paddingTop: 3.5}]}>💊</Text>
                </View>
              </Pressable>

              {/*  */}
              <Pressable onPress={()=>{DataHandler.set('cabinet', cabinet); router.navigate("/(cabinet)/cabinetmembers");}}>
                <View style={[styles.plusMinusButton, {backgroundColor: "#90e665"}]}>
                  <Text style={[styles.plusMinusText, {paddingTop: 5.5}]}>{getFamilyEmoji()}</Text>
                </View>
              </Pressable> 
              
              {/*  */}
              <View style={{alignContent: 'center', justifyContent: 'space-evenly', flexDirection: 'row' ,flexGrow: 1, minWidth: "50%", maxWidth: "60%"}}>
                <Text style={styles.text}>{cabinet.medicineCabinetName.slice(0,12) + (cabinet.medicineCabinetName.length > 12 ? "..." : "")}</Text>
                {/* deprecated ADD PERSON TO CABINET button */}
                {/*cabinet.isCreator &&
                <Pressable onPress={()=>{DataHandler.setState("medicineCabinetName", cabinet.medicineCabinetName); router.navigate("/(cabinet)/addperson");}}>
                  <Text style={[styles.plusMinusText]}>➕</Text>
                </Pressable>*/}
              </View>
      
            </View>
          </View>

        </Pressable>
      )
    }

  // MAIN PAGE LAYOUT
  return (    
    <View style={{backgroundColor: backgroundColorMain, flex: 1}}>
        <View style={{flex: 1}}>
          {/* page header */}
          {MrPillLogo(0.5)}
          <View style={styles.pagetop}>

            {/* page title */}
            <Text style={{lineHeight: 35, textAlign: 'center', fontSize: 24, textDecorationLine: 'underline', fontWeight: 'bold', marginTop: 8}}>
              ארונות תרופות שלי:
            </Text>

            {/* cabinet list */}
            <ParallaxScrollView backgroundColor={backgroundColorLight}>
              {myCabinets.map((cabinet, index) => renderCabinet(cabinet, index))}
              {myCabinets.length == 0 && <Text style={{fontSize: 20, color: "#FF0000"}}>אין ארונות תרופות. נא הוסף תחילה ארון.</Text>}
            </ParallaxScrollView>

          </View>
        </View>

        {/* page bottom */}
        <View style={styles.pagebottom}>
          <View style={styles.row}>
            <AppHomeButton BackgroundColor={backgroundColorLight} BorderColor={borderColor} ButtonContent={strFC("הוסף ארון חדש")} ButtonAction={()=>{router.navigate('/(cabinet)/addcabinet')}}/>
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
    padding: 5,
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
    lineHeight: 35,
    fontSize: 20,
    color: '#000',
  },
  reminderBox: {
    backgroundColor: backgroundColorMain,
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
    borderColor: backgroundColorLight,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 12,
  },
  plusMinusText: {
    fontSize: 25,
    lineHeight: 35,
    fontWeight: 'bold',
  }
});

export default MyCabinets;
