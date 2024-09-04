import React from 'react';
import { StyleSheet, View, Text } from 'react-native';
import { Asset } from 'expo-asset';
import { WebView } from 'react-native-webview';

const PdfViewer = () => {
    const pdfAsset = Asset.fromModule(require('./assets/your-file.pdf'));
    const pdfUri = pdfAsset.localUri;

    return (
        <View style={styles.container}>
            {pdfUri ? (
                <WebView
                    source={{ uri: pdfUri }}
                    style={styles.webview}
                />
            ) : (
                <Text>Loading...</Text>
            )}
        </View>
    );
};

const styles = StyleSheet.create({
    container: {
        flex: 1,
    },
    webview: {
        flex: 1,
    },
});

export default PdfViewer;