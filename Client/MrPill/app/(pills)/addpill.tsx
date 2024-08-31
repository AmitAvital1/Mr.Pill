import React, { useEffect, useState } from 'react';
import { SafeAreaView, StyleSheet, TextInput, View, Text, Pressable, Alert, TouchableOpacity } from 'react-native';

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
  const [scanned, setScanned] = useState<boolean>(false);
  const [cameraVisible, setCameraVisible] = useState<boolean>(false);

  useEffect(() => {
    (async () => {
      const { status } = await BarCodeScanner.requestPermissionsAsync();
      setHasPermission(status === 'granted');
    })();
  }, []);

  useEffect(() => {

    if (isRequestSent) return;

    setIsRequestSent(true);

    const sendGetCabinetsRequest = async () => {
      if (await RequestHandler.sendRequest('getMyCabinets')) {
        setCabinets(JSON.parse(RequestHandler.getResponse().request._response));
      }
    };

    sendGetCabinetsRequest();
  }, [isRequestSent]);


  const sendPostMedicineToCabinetRequest = async () => {
    
    DataHandler.setState('medicineCabinetName', cabinets[cabSelection].medicineCabinetName);
    DataHandler.setState('medicationBarcode', number);
    
    if (await RequestHandler.sendRequest('addPill')) {
      return true;
    }
    return false;
  
  };

  async function handleButtonPress() {
    const response = await sendPostMedicineToCabinetRequest();

    if (response) {
      router.replace('/(home)/home');
    } else {
      setIsRequestSent(true);
    }
  }

  const handleBarCodeScanned = ({ type, data }: BarCodeEvent) => {
    setScanned(true);
    onChangeNumber(data);
    setCameraVisible(false);
    Alert.alert("סריקת הברקוד הושלמה בהצלחה!\nאנא בחר ארון והמשך להוספת התרופה", `ברקוד: ${data}`, [{ text: "המשך" }]);
  };

  const toggleCamera = () => {
    setCameraVisible(!cameraVisible);
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
            <ThemedText style={{ fontSize: 22, marginHorizontal: 20, textAlign: 'center' }}>
              {cabinet.medicineCabinetName}
            </ThemedText>
          </View>
        </View>
      </Pressable>
    );
  };

  if (hasPermission === null) {
    return <Text>Requesting camera permission...</Text>;
  }
  if (hasPermission === false) {
    return <Text>No access to camera</Text>;
  }

  return (
    <SafeAreaView style={{ backgroundColor: backgroundColorMain, flex: 1 }}>
      <View style={{ minHeight: 180 }}>
        {MrPillLogo(1)}
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

      {!cameraVisible && (
        <TouchableOpacity style={styles.scanButton} onPress={toggleCamera}>
          <Text style={styles.scanButtonText}>סרוק ברקוד התרופה 📷</Text>
        </TouchableOpacity>
      )}

      <TextInput
        style={[styles.input, { backgroundColor: scanned ? 'lightgreen' : backgroundColorLight }]}
        onChangeText={onChangeNumber}
        value={number}
        placeholder="מספר ברקוד של תרופה"
        keyboardType="default"
        textAlign='center'
        editable={!scanned} // Allow manual input if barcode is not scanned
      />

      <View style={{ flexGrow: 1, minHeight: 160 }}>
        <View style={styles.pagetop}>
          <ThemedText style={{ textAlign: 'center', fontSize: 24, fontWeight: 'bold', marginTop: 10 }}>
            אנא בחר ארון להוספת התרופה:{"\n"}
          </ThemedText>
          {cabinets.length < 1 &&
          <ThemedText style={{ textAlign: 'center', fontSize: 20, fontWeight: 'bold', color: "#FF0000" }}>
          לא נמצאו ארונות למשתמש. אנא הוסף תחילה ארון אחד לפחות.{"\n"}
          </ThemedText>
          }
          <ParallaxScrollView backgroundColor={backgroundColorLight}>
            {cabinets.map((cabinet, index) => renderCabinet(cabinet, index))}
          </ParallaxScrollView>
        </View>
      </View>

      <View style={styles.pagebottom}>
        <View style={styles.row}>
          <AppHomeButton BackgroundColor={backgroundColorLight} BorderColor={borderColor} ButtonContent={strFC("הוסף תרופה לארון")} ButtonAction={handleButtonPress} />
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
    borderWidth: 0,
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
  input: {
    height: 60,
    margin: 8,
    borderWidth: 2,
    borderColor: borderColor,
    padding: 10,
    borderRadius: 12,
    fontSize: 25,
  },
  scanButton: {
    backgroundColor: '#ff7f7f',
    padding: 15,
    borderRadius: 30,
    borderWidth: 2,
    borderColor: '#ff5c5c',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    marginVertical: 10,
    marginHorizontal: 20,
    elevation: 5, // Adds shadow for better visibility
  },
  scanButtonText: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
    marginLeft: 10,
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
    backgroundColor: 'rgba(0,0,0,0.5)',
    padding: 10,
    borderRadius: 50,
    zIndex: 1001, // Ensure it is on top of camera view
  },
});

export default AddPillScreen;
