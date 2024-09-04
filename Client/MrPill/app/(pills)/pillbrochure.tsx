import React from 'react';
import { View, StyleSheet, Text } from 'react-native';
import { WebView } from 'react-native-webview';
import DataHandler from '@/DataHandler';

const PillBrochure = () => {
  const pdfUrl = DataHandler.getState("pdfURL");
  
  return (
    <View style={styles.container}>
      <Text style={{fontSize: 30, textAlign: 'center', marginTop: "40%"}}>הורדת עלון התרופה החלה</Text>
      {pdfUrl && <WebView source={{ uri: pdfUrl }} style={{ flex: 1 }}/>}
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
});

export default PillBrochure;