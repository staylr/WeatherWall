<?xml version="1.0" encoding="ISO-8859-1"?>
<StyledLayerDescriptor version="1.0.0" 
 xsi:schemaLocation="http://www.opengis.net/sld StyledLayerDescriptor.xsd" 
 xmlns="http://www.opengis.net/sld" 
 xmlns:ogc="http://www.opengis.net/ogc" 
 xmlns:xlink="http://www.w3.org/1999/xlink" 
 xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <!-- a Named Layer is the basic building block of an SLD document -->
  <NamedLayer>
    <Name>RASP_Wind</Name>
    <UserStyle>
      <Title>Wind Speed knots</Title>
      <Abstract></Abstract>
      <FeatureTypeStyle>
        <Rule>
          <Name>WStar_Colour</Name>
          <Title>8 Colour Red Map</Title>
          <Abstract></Abstract>
          <RasterSymbolizer>
            <Opacity>1.0</Opacity>
            <ColorMap extended="true">
              <ColorMapEntry color="#053061" quantity="5" />
              <ColorMapEntry color="#2166ac" quantity="10" />
              <ColorMapEntry color="#4393c3" quantity="15" />
              <ColorMapEntry color="#92c5de" quantity="20" />
              <ColorMapEntry color="#d1e5f0" quantity="25" />
              <ColorMapEntry color="#fddbc7" quantity="30" />
              <ColorMapEntry color="#f4a582" quantity="35" />
			  <ColorMapEntry color="#d6604d" quantity="35" />
			  <ColorMapEntry color="#b2182b" quantity="40" />
              <ColorMapEntry color="#67001f" quantity="255" />
            </ColorMap>
          </RasterSymbolizer>
        </Rule>
      </FeatureTypeStyle>
    </UserStyle>
  </NamedLayer>
</StyledLayerDescriptor>
