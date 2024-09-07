import React from 'react';
import { View, StyleSheet, Text, Pressable } from 'react-native';
import { WebView } from 'react-native-webview';
import DataHandler from '@/DataHandler';
import { router } from 'expo-router';
import { MrPillLogo } from '@/components/MrPillLogo';

const PillBrochure = () => {
  const pdfUrl = DataHandler.getState("pdfURL");
  
  return (
    <View style={styles.container}>
      <View style={{margin: 20}}/>
      {MrPillLogo(0.75)}
      <View style={{margin: 20}}/>
      <Text style={styles.text}>הורדת עלון התרופה החלה!</Text>
      <View style={{margin: 30}}/>
      <Text style={styles.text}>(לחץ על מר-פיל כדי לחזור חזרה)</Text>
      {pdfUrl && <WebView source={{ uri: pdfUrl }} style={{ flex: 1 }}/>}

      <Pressable style={styles.container} onPress={()=>router.dismiss()}>
        <Text style={styles.text}></Text>
      </Pressable>

    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    backgroundColor: '#ddc',
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  text: {
    fontSize: 30,
    textAlign: 'center',
  }
});

export default PillBrochure;