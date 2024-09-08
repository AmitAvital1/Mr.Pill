import React from 'react';
import { router } from 'expo-router';
import { ThemedText } from '@/components/ThemedText';
import { View, StyleSheet, Image, Pressable, Alert, Text } from 'react-native';
import { AppHomeButton } from "@/components/AppHomeButton";
import { MrPillLogo } from '@/components/MrPillLogo';
import { strFC } from "@/components/strFC";
import DataHandler from '@/DataHandler';

import RequestHandler from '@/RequestHandler';

const backgroundColorLight = "#cbc4ff";
const backgroundColorMain = "#c8e3e6";
const borderColor = "#02005a55";
const redButtonColor = "#f58e97";
const greenButtonColor = "lightgreen";

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
    medicineCabinetName: string;
    brochurePath: string;
}

const SinglePillPage: React.FC = () => {

  const pill: Pill = DataHandler.get('pill');

  const [screenUpdated, setScreenUpdated] = React.useState<boolean>();
  const [newDate, setNewDate] = React.useState<string>(pill.validity || new Date().toISOString());
  const [isEditEnabled, setIsEditEnabled] = React.useState<boolean>(false);
  const [isDateInputEnabled, setIsDateInputEnabled] = React.useState<boolean>(false);

  const changeDay = (increase: boolean) => {
    const result = new Date(newDate);
    result.setDate(result.getDate() + (increase? 1 : -1));
    setNewDate(result.toISOString());
  }

  const changeMonth = (increase: boolean) => {
    const result = new Date(newDate);
    result.setMonth(result.getMonth() + (increase? 1 : -1));
    setNewDate(result.toISOString());
  }

  const changeYear = (increase: boolean) => {
    const result = new Date(newDate);
    result.setFullYear(result.getUTCFullYear() + (increase? 1 : -1));
    setNewDate(result.toISOString());
  }

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

    Alert.alert(
        "הורדת עלון תרופה", //title
        "האם ברצונך להוריד את עלון התרופה?", // message
        [
          {
            text: "ביטול",
            onPress: () => {},
            style: "cancel", 
          },
          {
            text: "אישור",
            onPress: () => {
                DataHandler.setState("pdfURL", pill.brochurePath);
                router.push("/(pills)/pillbrochure");
            },
          }
        ],
        { cancelable: true } 
      );

  }

  const handleDateButtonPress = () => {
    if (!isDateInputEnabled) {
      setIsDateInputEnabled(true);
    } else {
      sendChangeDateRequest();
    }
  }

  const sendDeletePillRequest = async () => {
    
    DataHandler.setState("medicineCabinetName", pill.medicineCabinetName);
    DataHandler.setState("pillId", pill.id.toString())
    
    if (await RequestHandler.sendRequest("deletePill")) {
      Alert.alert("תרופה נמחקה בהצלחה!");
      router.dismiss()
    } else if (RequestHandler.getResponse().request.status === 409) {
      Alert.alert("אין אפשרות למחוק תרופה בעלת תזכורות פעילות.")
    } else {
      console.log(RequestHandler.getRequest());
      Alert.alert("שגיאה במחיקת התרופה");
    }

  }

  const sendChangeDateRequest = async () => {

    DataHandler.setState("pillId", pill.id.toString());
    DataHandler.setState("pillDate", newDate);

    if (await RequestHandler.sendRequest("updatePillDate")) {
      Alert.alert("תוקף עודכן בהצלחה!");
      pill.validity = newDate;
    } else {
      console.log(RequestHandler.getRequest());
      Alert.alert("שגיאה בעת עדכון תאריך התרופה");
    }
    setIsDateInputEnabled(false);
    setScreenUpdated(!screenUpdated);
  }

  // MAIN PAGE LAYOUT
  return (    
    <View style={{backgroundColor: backgroundColorMain, flex: 1}}>
        <View style={{minHeight: "10%", marginTop: 5}}>
          {MrPillLogo(0.3)}
          <Pressable style={{alignSelf: 'flex-start', marginLeft: 15, minHeight: 50, minWidth: 100, maxWidth: 250}} onPress={handlePillImagePress}>
            <Text style={styles.plusMinusText}>💾</Text>
          </Pressable>
        </View>
        
        <View style={styles.pagetop}>
            <View style={styles.imageContainer}>
              <Text style={styles.text}>{pill.hebrewName}</Text>
                <Image source={{uri: pill.imagePath}} style={styles.image} resizeMode="stretch"/>
              <Text style={styles.text}>{pill.hebrewDescription}</Text>
            </View>
            <View style={{flexDirection: 'row'}}>
              
              <Pressable style={{marginHorizontal: 10, alignItems: 'center', justifyContent: 'center', alignContent: 'center', borderColor: "#777", borderWidth: 3, backgroundColor: backgroundColorMain, minHeight: 100, minWidth: 100, borderRadius: 999}} onPress={()=>{setIsEditEnabled(!isEditEnabled); setIsDateInputEnabled(false)}}>
                <Text style={{color: "#333", fontSize: 50, lineHeight: 60, fontWeight: 'bold' }}>✏</Text>
              </Pressable>
              
              <View>
                <Text style={styles.text}>מתוך ארון התרופות:{"\n"}{pill.medicineCabinetName}</Text>
                {pill.validity && <Text style={styles.text}>תוקף: {pill.validity.slice(0, 10)}</Text>}
              </View>
            </View>


            {// edit pill inputs
            isEditEnabled &&
            <View style={{minHeight: "10%", flex: 1, flexDirection: 'row'}}>

                {// delete pill
                !isDateInputEnabled &&
                <Pressable style={{marginHorizontal: 10, justifyContent: 'center', alignContent: 'center', borderColor: "#777", borderWidth: 3, backgroundColor: redButtonColor, minHeight: 100, minWidth: 100, borderRadius: 999}} onPress={sendDeletePillRequest}>
                  <Text style={styles.text}>מחק{'\n'}תרופה</Text>
                </Pressable>}
                
                {// edit date inputs
                isDateInputEnabled &&
                <View style={{flexDirection: 'row'}}>
                  
                  <View style={{gap: 10}}>
                    <Text style={styles.text}>שנה</Text>
                    <Pressable style={styles.greenButton} onPress={()=>changeYear(true)}>
                      <Text style={styles.text}>+</Text>
                    </Pressable>

                    <Pressable style={styles.redButton} onPress={()=>changeYear(false)}>
                      <Text style={styles.text}>-</Text>
                    </Pressable>   

                  </View>
                  <View style={{gap: 10}}>
                    <Text style={styles.text}>חודש </Text>
                    <Pressable style={styles.greenButton} onPress={()=>changeMonth(true)}>
                      <Text style={styles.text}>+</Text>
                    </Pressable>

                    <Pressable style={styles.redButton} onPress={()=>changeMonth(false)}>
                      <Text style={styles.text}>-</Text>
                    </Pressable>

                  </View>
                  <View style={{gap: 10}}>
                    <Text style={styles.text}>יום </Text>
                    <Pressable style={styles.greenButton} onPress={()=>changeDay(true)}>
                      <Text style={styles.text}>+</Text>
                    </Pressable>

                    <Pressable style={styles.redButton} onPress={()=>changeDay(false)}>
                      <Text style={styles.text}>-</Text>
                    </Pressable>

                  </View>
                </View>}

                <Pressable style={[{backgroundColor: isDateInputEnabled ? greenButtonColor : backgroundColorMain}, {marginHorizontal: 10, justifyContent: 'center', alignContent: 'center', borderColor: "#777", borderWidth: 3, backgroundColor: backgroundColorMain, minHeight: 100, minWidth: 100, borderRadius: 999}]} onPress={handleDateButtonPress}>
                  <Text style={styles.text}>{isDateInputEnabled ? "אישור\nהתוקף\nהחדש" : "עדכן\nתוקף"}</Text>
                </Pressable>

            </View>}

            {isEditEnabled && isDateInputEnabled &&
              <Text style={[styles.text, {color: "#581d22"}]}>התוקף החדש: {newDate.slice(0,10)}</Text>
            }

            {!isEditEnabled && <View style={{flex: 1}}/>}

            {!isDateInputEnabled &&
            <Text style={styles.text}>מספר התרופות שנותרו:  <Text style={[styles.text, {lineHeight: 50, fontSize: 40, color: pill.numberOfPills < 6 ? "red" : (pill.numberOfPills < 11 ? "yellow" : "green")}]}>{pill.numberOfPills}</Text></Text>
            }
        </View>
        
        
        {!isDateInputEnabled &&
        <View style={styles.pagebottom}>
            <View style={styles.row}>
                <AppHomeButton BackgroundColor={backgroundColorLight} BorderColor={borderColor} ButtonContent={strFC("➕", 50)} ButtonAction={()=>{handlePlusButtonPress()}}/>
            </View>
            <View style={styles.row}>
                <AppHomeButton BackgroundColor={backgroundColorLight} BorderColor={borderColor} ButtonContent={strFC("➖", 50)} ButtonAction={()=>{handleMinusButtonPress()}}/>
            </View>
        </View>}
        
        {isDateInputEnabled && <View style={{margin: 10}} />}

    </View>
  );
};

const styles = StyleSheet.create({
  pagetop: {
    flex: 1,
    marginTop: 5,
    justifyContent: 'flex-start',
    alignItems: 'center',
    backgroundColor: backgroundColorLight,
    borderRadius: 20,
    borderColor: borderColor,
    minHeight: "50%",
    marginHorizontal: 15,
    paddingBottom: 5,
    paddingHorizontal: 5,
    gap: 5,
    elevation: 3,
  },
  greenButton: {
    marginHorizontal: 10,
    justifyContent: 'center',
    alignContent: 'center',
    borderColor: "#777",
    borderWidth: 3,
    backgroundColor: greenButtonColor,
    minHeight: 50,
    minWidth: 50,
    borderRadius: 999
  },
  redButton: {
    marginHorizontal: 10,
    justifyContent: 'center',
    alignContent: 'center',
    borderColor: "#777",
    borderWidth: 3,
    backgroundColor: redButtonColor,
    minHeight: 50,
    minWidth: 50,
    borderRadius: 999
  },
  pagebottom: {
    flex: 1,
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    borderRadius: 20,
    marginHorizontal: 15,
    padding: 5,
    minHeight: "15%",
    maxHeight: "25%"
  },
  row: {
    flex: 1,
    minHeight: 5,
    justifyContent: 'center',
    alignItems: 'center',
    flexDirection: 'row',
  },
  text: {
    lineHeight: 34,
    fontSize: 24,
    fontWeight: 'bold',
    color: '#000',
    textAlign: 'center',
  },
  plusMinusText: {
    lineHeight: 54,
    fontSize: 44,
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
    height: "90%",
    width: "90%",
  }, 
  imageContainer: {
    margin: 8,
    flexGrow: 1,
    width: "100%",
    borderRadius: 20,
    borderColor: "#747474",
    backgroundColor: "#bdddf3",
    justifyContent: 'center',
    alignContent: 'center',
    overflow: 'hidden',
    elevation: 4,
    gap: 3,
  },
});

export default SinglePillPage;