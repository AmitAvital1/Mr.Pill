import React, { useEffect } from 'react';
import {SafeAreaView, StyleSheet, TextInput, View, Text, Button, Pressable} from 'react-native';
import axios from 'axios';
import DataHandler from '@/DataHandler'
import { router } from 'expo-router';
import { MrPillLogo } from '@/components/MrPillLogo';
import { ThemedText } from '@/components/ThemedText';
import ParallaxScrollView from '@/components/ParallaxScrollView';
import { AppHomeButton } from '@/components/AppHomeButton';
import { strFC } from '@/components/strFC';

type Cabinet = {
  id: number,
  medicineCabinetName: string,
  creatorId: number,
};


const backgroundColorLight = "#ffd8d8";
const backgroundColorMain = "#ffdf7e";
const borderColor = "#882c2c";

const AddPillScreen = () => {

  const user = DataHandler.getUser()

  const [number, onChangeNumber] = React.useState('');
  const [cabinets, setCabinets] = React.useState<Cabinet[]>([]);
  const [cabSelection, setCabSelection] = React.useState<number>(-1);
  const [isRequestSent, setIsRequestSent] = React.useState<boolean>(false);

  useEffect(() => {
      
    if(isRequestSent) return;
    setIsRequestSent(true);
  
    const sendGetCabinetsRequest = async () => {

      // bug when adding medication and then trying to get all user medications
      try {
        console.log(user.Token)
        const request = {
          method: 'get',
          url: "http://10.0.2.2:5194/user/cabinet",
          headers: {
            "Authorization": "Bearer " + user.Token, 
          },
          data: {
          }
        }

        const response = await axios(request);
  
        console.log("full: " + response.request._response);
        console.log("status: " + response.request.status);
        
        if (response.request.status == 200) {
          setCabinets(JSON.parse(response.request._response));
          console.log(cabinets);
          return true;
        } else {
          DataHandler.expireSession();
        }
  
      } catch (error) {
        console.error("Error fetching data:", error);
        DataHandler.expireSession();
      }

    }
    sendGetCabinetsRequest();
  })

  const sendPostMedicineToCabinetRequest = async () => {

    // bug when adding medication and then trying to get all user medications
    try {
      console.log(user.Token)
      const request = {
        method: 'post',
        url: "http://10.0.2.2:5194/user/medications?medicineCabinetName=" + cabinets[cabSelection].medicineCabinetName,
        headers: {
          "Authorization": "Bearer " + user.Token,
        },
        data: {
          MedicationBarcode: number,
          Privacy: false
        }
      }

      const response = await axios(request);

      console.log("full: " + response.request._response);
      console.log("status: " + response.request.status);
      
      if (response.request.status == 200) {
        return true;
      } else {
        
      }

    } catch (error) {
      console.error("Error fetching data:", error);
      DataHandler.expireSession();
    }

  }

  async function handleButtonPress () { 
    const response = await sendPostMedicineToCabinetRequest();

    if (response) {
      router.replace('/(home)/home');
    }
    else {
      setIsRequestSent(true);
    }

  }

  const renderCabinet = (cabinet: Cabinet, position: number) => {
    const isSelected = position === cabSelection;
    const color = isSelected ? "lightgreen" : backgroundColorMain;

    return (
      <Pressable key={position} onPress={() => {setCabSelection(position);}}>

        <View style={[styles.reminderBox, { backgroundColor: color, minHeight: 50 }]}>
          <View style={{flexDirection: 'row'}}>
            <ThemedText style={{fontSize: 22, marginHorizontal: 20, textAlign: 'center' }}>
              {cabinet.medicineCabinetName}
            </ThemedText>
          </View>
        </View>
        
      </Pressable>
    );
  };

  return (
    <SafeAreaView style={{backgroundColor: backgroundColorMain, flex: 1}}>
      
      <View style={{minHeight: 180}}>
          {MrPillLogo(1)}
      </View>

      <TextInput
        style={styles.input}
        onChangeText={onChangeNumber}
        value={number}
        placeholder="מספר ברקוד של תרופה"
        keyboardType="default"
        textAlign='center'
        onEndEditing={()=>{}}
      />

      <View style={{flexGrow: 1, minHeight: 160,}}>
        <View style={styles.pagetop}> 
            <ThemedText style={{textAlign: 'center', fontSize: 24, fontWeight: 'bold', marginTop: 10}}>
                אנא בחר ארון להוספת התרופה:{"\n"}
            </ThemedText>

            <ParallaxScrollView backgroundColor={backgroundColorLight}>
              {cabinets.map((cabinet, index) => renderCabinet(cabinet, index))}
            </ParallaxScrollView>

        </View>
      </View>

      <View style={styles.pagebottom}>
        <View style={styles.row}>
            <AppHomeButton BackgroundColor={backgroundColorLight} BorderColor={borderColor} ButtonContent={strFC("הוסף תרופה לארון")} ButtonAction={handleButtonPress}/>
        </View>
      </View>

    </SafeAreaView>
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
    marginHorizontal: 15,
    marginVertical: 20,
    padding: 5,
    minHeight: 180,
    maxHeight: 180,
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
    borderWidth: 2,
    borderColor: borderColor,
    borderRadius: 12,
    flex: 1,
    justifyContent: 'center',
    alignContent: 'center',
    alignItems: 'center',
    paddingHorizontal: 15,
    paddingVertical: 5,
    minWidth: 300,
  },
  plusMinusButton: {
    minWidth: 50,
    minHeight: 50,
    justifyContent: 'center',
    alignItems: 'center',
  },
  plusMinusText: {
    fontSize: 30,
    fontWeight: 'bold',
    position: 'absolute',
  }, 
  input: {
    backgroundColor: backgroundColorLight,
    height: 60,
    margin: 8,
    borderWidth: 2,
    borderColor: borderColor,
    padding: 10,
    borderRadius: 12,
    fontSize: 25,
  },
});


export default AddPillScreen;