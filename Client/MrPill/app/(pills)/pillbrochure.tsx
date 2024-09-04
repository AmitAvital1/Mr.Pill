import React from 'react';
import { View, StyleSheet } from 'react-native';
import { WebView } from 'react-native-webview';
import DataHandler from '@/DataHandler';

const PillBrochure = () => {
  const pdfUrl = DataHandler.getState("pdfURL");

  return (
    <View style={styles.container}>
      {pdfUrl && <WebView source={{ uri: pdfUrl }} style={{ flex: 1 }} />}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
});

export default PillBrochure;