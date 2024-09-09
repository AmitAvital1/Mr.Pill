import React, { useEffect, useState } from 'react';
import { SafeAreaView, StyleSheet, TextInput, View, Text, Pressable, Alert, TouchableOpacity, ScrollView } from 'react-native';

import DataHandler from '@/DataHandler';
import { router } from 'expo-router';
import { MrPillLogo } from '@/components/MrPillLogo';
import { ThemedText } from '@/components/ThemedText';
import ParallaxScrollView from '@/components/ParallaxScrollView';
import { AppHomeButton } from '@/components/AppHomeButton';
import { strFC } from '@/components/strFC';
import { BarCodeScanner, BarCodeEvent } from 'expo-barcode-scanner';
import { MaterialIcons } from '@expo/vector-icons';
import RequestHandler from '@/RequestHandler';

type Cabinet = {
  id: number,
  medicineCabinetName: string,
  creatorId: number,
};

const backgroundColorLight = "#ffd8d8";
const backgroundColorMain = "#ffdf7e";
const borderColor = "#882c2c";

const AddPillScreen = () => {

  const [number, onChangeNumber] = useState('');
  const [cabinets, setCabinets] = useState<Cabinet[]>([]);
  const [cabSelection, setCabSelection] = useState<number>(-1);
  const [isRequestSent, setIsRequestSent] = useState<boolean>(false);
  const [hasPermission, setHasPermission] = useState<boolean | null>(null);
  const [isPrivate, setIsPrivate] = useState<boolean>(true);
  const [scanned, setScanned] = useState<boolean>(false);
  const [cameraVisible, setCameraVisible] = useState<boolean>(false);

  useEffect(() => {
    const checkCameraPermissions = async () => {
      const { status } = await BarCodeScanner.getPermissionsAsync();
      setHasPermission(status === 'granted');
    };

    checkCameraPermissions();
  }, []);

  useEffect(() => {

    const sendGetCabinetsRequest = async () => {
      if (await RequestHandler.sendRequest('getMyCabinets')) {
        setCabinets(JSON.parse(RequestHandler.getResponse().request._response));
      }
    };

    sendGetCabinetsRequest();
  }, []);

  const sendPostMedicineToCabinetRequest = async () => {

    setIsRequestSent(true);

    DataHandler.setState('medicineCabinetName', cabinets[cabSelection].medicineCabinetName);
    DataHandler.setState('medicationBarcode', number);
    DataHandler.setFlag("privatePill", isPrivate);

    if (await RequestHandler.sendRequest('addPill')) {
      setIsRequestSent(false);
      return true;
    }

    setIsRequestSent(false);
    return false;

  };

  async function handleButtonPress() {
    if (cabSelection == -1) return;
    const response = await sendPostMedicineToCabinetRequest();

    if (response) {
      Alert.alert("תרופה נוספה לארון בהצלחה!");
      router.replace('/(home)/home');
    } else {
      Alert.alert("שגיאה בהוספת התרופה.");
    }
  }

  const handleBarCodeScanned = ({ type, data }: BarCodeEvent) => {
    setScanned(true);
    onChangeNumber(data);
    setCameraVisible(false);
    Alert.alert("סריקת הברקוד הושלמה בהצלחה!\nאנא בחר ארון והמשך להוספת התרופה", `ברקוד: ${data}`, [{ text: "המשך" }]);
  };

  const toggleCamera = async () => {
    if (hasPermission === null || hasPermission === false) {
      const { status } = await BarCodeScanner.requestPermissionsAsync();
      if (status === 'granted') {
        setHasPermission(true);
        setCameraVisible(true);
      } else {
        Alert.alert("אין גישה למצלמה", "כדי לסרוק ברקוד, יש לאשר גישה למצלמה.", [{ text: "הבנתי" }]);

      }
    } else {
      setCameraVisible(true);
    }
    setScanned(false); // Reset scanned status when opening camera
  };

  const closeCamera = () => {
    setCameraVisible(false);
    setScanned(false); // Reset scanned status if closing camera
  };

  const renderCabinet = (cabinet: Cabinet, position: number) => {
    const isSelected = position === cabSelection;
    const color = isSelected ? "lightgreen" : backgroundColorMain;

    return (
      <Pressable key={position} onPress={() => { setCabSelection(position); }}>
        <View style={[styles.reminderBox, { backgroundColor: color, minHeight: 50 }]}>
          <View style={{ flexDirection: 'row' }}>
            <Text style={{ fontSize: 22, marginHorizontal: 20, textAlign: 'center' }}>
              {cabinet.medicineCabinetName}
            </Text>
          </View>
        </View>
      </Pressable>
    );
  };

  return (
    <ScrollView style={{ backgroundColor: backgroundColorMain, flex: 1 }}>
      <View style={{justifyContent: 'center', minHeight: 90, marginBottom: 10}}> 
      {MrPillLogo(0.5)}
      </View>
      {cameraVisible && (
        <View style={styles.cameraContainer}>
          <BarCodeScanner
            onBarCodeScanned={scanned ? undefined : handleBarCodeScanned}
            style={StyleSheet.absoluteFillObject}
          />
          <TouchableOpacity style={styles.closeButton} onPress={closeCamera}>
            <MaterialIcons name="close" size={24} color="white" />
          </TouchableOpacity>
        </View>
      )}

      <Text style={{ lineHeight: 30, textAlign: 'center', fontSize: 18, fontWeight: 'bold', marginHorizontal: 30, marginTop: 5, }}>
        הכנס ידנית מספר ברקוד של תרופה או לחץ בכדי לסרוק באמצעות המצלמה.
      </Text>

      <TextInput
        style={[styles.input, { backgroundColor: scanned ? 'lightgreen' : backgroundColorLight }]}
        onChangeText={onChangeNumber}
        value={number}
        placeholder="מספר ברקוד של תרופה"
        keyboardType="default"
        textAlign='center'
        editable={!scanned} // Allow manual input if barcode is not scanned
      />

      {!cameraVisible && (
        <TouchableOpacity style={styles.scanButton} onPress={toggleCamera}>
          <Text style={styles.scanButtonText}>סרוק באמצעות המצלמה 📷</Text>
        </TouchableOpacity>
      )}

      <View style={{ flexGrow: 1, minHeight: 160 }}>
        <View style={styles.pagetop}>
          <Text style={{ lineHeight: 30, textAlign: 'center', fontSize: 24, fontWeight: 'bold', marginTop: 10 }}>
           בחר ארון להוספת התרופה:
          </Text>
          {cabinets.length < 1 &&
          <Text style={{ textAlign: 'center', fontSize: 20, fontWeight: 'bold', color: "#FF0000" }}>
              לא נמצאו ארונות למשתמש. אנא הוסף תחילה ארון אחד לפחות.{"\n"}
          </Text>
          }
          <ParallaxScrollView backgroundColor={backgroundColorLight}>
            {cabinets.map((cabinet, index) => renderCabinet(cabinet, index))}
          </ParallaxScrollView>
        </View>
      </View>
      <Text style={styles.text}>בחר את מצב פרטיות התרופה:</Text>
      <View style={{flex: 0, flexDirection:'row', minHeight: 50, justifyContent: 'center', gap: 50, marginTop: 10}}>
        
        <Pressable onPress={()=>{setIsPrivate(false);}}>
          <View style={[styles.privacyButton, {backgroundColor: !isPrivate? "#a6fda3" : backgroundColorMain}]}>
            <Text style={styles.text}>קבוצתי</Text>
          </View>
        </Pressable>
        
        <Pressable onPress={()=>{setIsPrivate(true);}}>
          <View style={[styles.privacyButton, {backgroundColor: isPrivate? "#a6fda3" : backgroundColorMain}]}>
            <Text style={styles.text}>פרטי</Text>
          </View>
        </Pressable>
      </View>

      <View style={styles.pagebottom}>
        <View style={styles.row}>
          <AppHomeButton BackgroundColor={isRequestSent ? 'darkgrey' : 'lightgreen'} BorderColor={borderColor} ButtonContent={strFC("הוסף תרופה לארון")} ButtonAction={isRequestSent ? ()=>{} : handleButtonPress} />
        </View>
      </View>

    </ScrollView>
  );
};

const styles = StyleSheet.create({
  pagetop: {
    flex: 1,
    justifyContent: 'flex-start',
    alignItems: 'center',
    backgroundColor: backgroundColorLight,
    borderRadius: 20,
    borderWidth: 0,
    borderColor: borderColor,
    minHeight: 100,
    marginHorizontal: 15,
    paddingBottom: 5,
    elevation: 5,
  },
  
  privacyButton: {
    minHeight: 80,
    minWidth: 100,
    justifyContent: 'center',
    alignItems: 'center',
    borderWidth: 2,
    borderRadius: 10,
    borderColor: borderColor
  },

  pagebottom: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    borderRadius: 20,
    marginHorizontal: 15,
    padding: 5,
    minHeight: 180,
  },
  row: {
    flex: 1,
    minHeight: 5,
    justifyContent: 'center',
    alignItems: 'center',
    flexDirection: 'row',
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
    elevation: 2,
  },
  input: {
    alignSelf: 'center',
    height: 60,
    marginTop: 5,
    width: "90%",
    borderWidth: 2,
    borderColor: borderColor,
    padding: 10,
    borderRadius: 12,
    fontSize: 25,
    elevation: 5,
  },
  scanButton: {
    backgroundColor: '#41c052',
    padding: 15,
    borderRadius: 30,
    borderWidth: 2,
    borderColor: '#2b8037',
    margin: 10,
    alignItems: 'center',
    elevation: 5,
  },
  scanButtonText: {
    fontSize: 22,
    color: '#ffffff',
    fontWeight: 'bold',
  },
  text: {
    fontSize: 22,
    color: '#000',
    fontWeight: 'bold',
    textAlign: 'center',
  },
  cameraContainer: {
    ...StyleSheet.absoluteFillObject,
    backgroundColor: 'black',
    justifyContent: 'center',
    alignItems: 'center',
    zIndex: 1000, // Ensure it is on top of other components
  },
  closeButton: {
    position: 'absolute',
    top: 40,
    right: 20,
    backgroundColor: '#ff5c5c',
    padding: 10,
    borderRadius: 20,
    zIndex: 1001, // Ensure close button is above the camera view
  },
});

export default AddPillScreen;
